using System.Collections.Concurrent;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.DataAccess;
using Tradibit.Shared.Entities;
using Tradibit.Shared.Events;
using Tradibit.SharedUI;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Coins;
using Tradibit.SharedUI.DTO.Primitives;
using Tradibit.SharedUI.DTO.Scenarios;
using Z.EntityFramework.Plus;

namespace Tradibit.Api.Scenarios;

public class ScenarioWorker :
    INotificationHandler<AppInitEvent>,
    IRequestHandler<StrategyChangeStatusEvent>,
    IRequestHandler<StartBackTestStrategyEvent>,
    IRequestHandler<KlineUpdateEvent>,
    IRequestHandler<KlineHistoryUpdateEvent>
{
    private readonly IMediator _mediator;
    private readonly TradibitDb _db;

    private static readonly ConcurrentDictionary<(PairIntervalKey PairIntervalKey, Guid StrategyId), Scenario> ActiveScenariosDict = new();
    private static readonly ConcurrentDictionary<(PairIntervalKey PairIntervalKey, Guid StrategyId), Scenario> ReplyHistoryScenariosDict = new();

    public ScenarioWorker(IMediator mediator, TradibitDb db)
    {
        _mediator = mediator;
        _db = db;
    }
    
    // var balances = await _mediator.Send(new GetBalancesEvent(request.UserId, Currency.USDT), cancellationToken);
    //
    // var usdt = balances.FirstOrDefault(x => x.Key == Currency.USDT).Value;
    // if (usdt < 100)
    //     throw new Exception("No USDT funds found or USDT funds < $100");
    //
    // var totalFundsBtc = balances.Aggregate(0m, (s, balance) => s + balance.Value);
    // var totalUSDT = _candlesProvider.BtcValue * totalFundsBtc;
    // var requiredUSDT = totalUSDT * scenario.DepositPercent / 100;
    //
    // if (usdt < requiredUSDT)
    //     throw new Exception($"Available USDT funds is less then required USDT for the scenario! " +
    //                         $"Available USDT: {usdt}. Required for scenario: {requiredUSDT}, scenario percent: {scenario.DepositPercent}%");
    //     
    //StartScenario(false, scenario, requiredUSDT, cancellationToken);
    
    public async Task<Unit> Handle(StrategyChangeStatusEvent request, CancellationToken cancellationToken)
    {
        if (request.IsEnabled) 
            await AddStrategyScenarios(request.StrategyId, isHistory: false, null, null, cancellationToken);
        else
            await RemoveStrategyScenarios(request.StrategyId, isHistory: false, null, null, cancellationToken);
        
        return Unit.Value;
    }

    public async Task<Unit> Handle(StartBackTestStrategyEvent request, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserByIdRequest(request.UserId), cancellationToken);
        user.HistoryUserState.Add(new UserState(request.Deposit, request.StrategyId));
        
        await AddStrategyScenarios(request.StrategyId, isHistory: true, request.Pairs, request.Intervals, cancellationToken);
        
        var pairs = request.Pairs ?? await _mediator.Send(new GetMostCapCoinsRequest(), cancellationToken);
        var intervals = request.Intervals ?? Constants.DefaultIntervals;
        
        await _mediator.Send(new ReplyHistoryEvent(request.HistorySpan, pairs, intervals), cancellationToken);

        await RemoveStrategyScenarios(request.StrategyId, isHistory: true, pairs, intervals, cancellationToken);
        return Unit.Value;
    }

    private async Task AddStrategyScenarios(Guid strategyId, bool isHistory, List<Pair> pairs, List<Interval> intervals, CancellationToken cancellationToken)
    {
        var strategy = await _db.Strategies.FindAsync(strategyId);
        var scenarios = (await ToPairIntervals(pairs, intervals, cancellationToken)).Select(pairIntervalKey => new Scenario
        {
            StrategyId = strategyId,
            PairIntervalKey = pairIntervalKey,
            CurrentStepId = strategy!.InitialStepId,
            UserVars = new Dictionary<string, decimal?>()
        }).ToList();
        await _db.BulkInsertAsync(scenarios, cancellationToken);

        var scenariosDict = isHistory ? ReplyHistoryScenariosDict : ActiveScenariosDict; 
        foreach (var scenario in scenarios)
            scenariosDict.TryAdd((scenario.PairIntervalKey, strategyId), scenario);
    }

    private async Task RemoveStrategyScenarios(Guid strategyId, bool isHistory, List<Pair> pairs, List<Interval> intervals, CancellationToken cancellationToken)
    {
        await _db.Scenarios.Where(x => x.StrategyId == strategyId)
            .DeleteAsync(cancellationToken);
        var scenariosDict = isHistory ? ReplyHistoryScenariosDict : ActiveScenariosDict;

        var pairIntervals = await ToPairIntervals(pairs, intervals, cancellationToken);
        foreach (var pairInterval in pairIntervals)
            scenariosDict.TryRemove((pairInterval, strategyId), out _);
    }

    private async Task<List<PairIntervalKey>> ToPairIntervals(List<Pair> pairs, List<Interval> intervals, CancellationToken cancellationToken)
    {
        pairs ??= await _mediator.Send(new GetMostCapCoinsRequest(), cancellationToken);
        intervals ??= Constants.DefaultIntervals;
        return pairs.SelectMany(_ => intervals, (pair, interval) => new PairIntervalKey(pair, interval)).ToList();
    }

    public async Task Handle(AppInitEvent notification, CancellationToken cancellationToken)
    {
        var scenarios = await _db.Scenarios.Where(s => s.Strategy.IsActive).ToListAsync(cancellationToken);
        foreach (var scenario in scenarios)
            ActiveScenariosDict.TryAdd((scenario.PairIntervalKey, scenario.StrategyId), scenario);
    }

    public async Task<Unit> Handle(KlineUpdateEvent e, CancellationToken cancellationToken)
    {
        foreach (var scenario in ActiveScenariosDict)
            await MoveNext(scenario.Value, e.QuoteIndicator, cancellationToken);

        return Unit.Value;
    }

    private async Task MoveNext(Scenario scenario, QuoteIndicator quoteIndicator, CancellationToken cancellationToken)
    {
        bool transited;
        do //could be multiple transition on 1 Kline update
        {
            transited = false;
            foreach (var transition in scenario.CurrentStep.Transitions)
            {
                if (!transition.Conditions.All(c => c.Meet(scenario, quoteIndicator)))
                    continue;

                await Task.WhenAll( transition.SuccessOperations); 
                scenario.CurrentStepId = transition.DestinationStepId;
                await _db.Save(scenario, cancellationToken);
                break;
            }
        } while (transited);
    }

    public async Task<Unit> Handle(KlineHistoryUpdateEvent e, CancellationToken cancellationToken)
    {
        if (!ReplyHistoryScenariosDict.TryGetValue((e.PairIntervalKey, e.StrategyId), out var scenario))
            return Unit.Value;
        
        await MoveNext(scenario, e.QuoteIndicator, cancellationToken);
        return Unit.Value;
    }
}
