using Skender.Stock.Indicators;
using Tradibit.Api.Services.Candles;
using Tradibit.Common.DTO;
using Tradibit.Common.Entities;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Scenarios;

public class Scenario
{
    private readonly CandlesProviderResolver _candlesProviderResolver;
    
    private readonly IUserBrokerService _userBrokerService;
    private ICandlesService _candlesService;
    private readonly User _user;
    
    public Strategy Strategy { get; set; }
    public ScenarioState State { get; set; }
    
    public bool IsActive { get; set; }
    public decimal DepositPercent { get; set; }

    public Scenario(CandlesProviderResolver candlesProviderResolver, ICurrentUserProvider currentUserProvider, IUserBrokerService userBrokerService)
    {
        _candlesProviderResolver = candlesProviderResolver;
        _candlesService = candlesProviderResolver(CandlesResolverEnum.Realtime);
        
        _userBrokerService = userBrokerService;
        _user = currentUserProvider.CurrentUser;
    }
    
    public async Task Start(CancellationToken cancellationToken = default)
    {
        var usdDeposit = await _userBrokerService.GetUsdtBalance(cancellationToken);
        State = new ScenarioState
        {
            DepositMoney = usdDeposit * DepositPercent
        };
        await _candlesService.SubscribeKlineHandler(Handler);
    }

    public async Task TestHistory(decimal deposit, TimeSpan historySpan, CancellationToken cancellationToken = default)
    {
        _candlesService = _candlesProviderResolver(CandlesResolverEnum.History);
        State = new ScenarioState
        {
            DepositMoney = deposit
        };
        await _candlesService.SubscribeKlineHandler(HistoryHandler);
        await ((HistoryCandlesService)_candlesService).StartProcessHistory(historySpan, cancellationToken);
    }

    private async Task HistoryHandler(Pair pair, Quote quote, Dictionary<IndicatorEnum, decimal?> indicators)
    {
        var exchangeFee = 0.001m;
        State.LastQuote = quote;
        State.LastIndicators = indicators;
        
        if (State.ActivePair is null && Strategy.BuyConditions.All(c => c.Meet(State)))
        {
            State.PositionMoney = (1 - exchangeFee) * State.DepositMoney * quote.Close;
            State.DepositMoney = 0;
            State.ActivePair = pair;
        }

        if (State.ActivePair == pair && Strategy.SellConditions.All(c => c.Meet(State)))
        {
            State.DepositMoney = (1 - exchangeFee) * State.PositionMoney / quote.Close;
            State.PositionMoney = 0;
            State.ActivePair = null;
        }
    }

    private async Task Handler(Pair pair, Quote quote, Dictionary<IndicatorEnum, decimal?> indicators)
    {
        State.LastQuote = quote;
        State.LastIndicators = indicators;
        
        if (State.ActivePair is null && Strategy.BuyConditions.All(c => c.Meet(State)))
        {
            State.PositionMoney = await _userBrokerService.Buy(pair, State.DepositMoney);
            State.DepositMoney = 0;
            State.ActivePair = pair;
        }

        if (State.ActivePair == pair && Strategy.SellConditions.All(c => c.Meet(State)))
        {
            State.DepositMoney = await _userBrokerService.Sell(pair, State.PositionMoney);
            State.PositionMoney = 0;
            State.ActivePair = null;
        }
    }
}

public class ScenarioState
{
    public decimal DepositMoney { get; set; }
    public decimal PositionMoney { get; set; }
    public Dictionary<string, decimal?> UserVars { get; set; }
        
    public Pair ActivePair { get; set; }
    
    public Quote LastQuote { get; set; }
    public Dictionary<IndicatorEnum, decimal?> LastIndicators { get; set; }
}

public class Strategy
{
    public List<Condition> BuyConditions { get; set; }
    public List<Condition> SellConditions { get; set; }
}
