using Binance.Net.Clients;
using Binance.Net.Enums;
using MediatR;
using Skender.Stock.Indicators;
using Tradibit.Common;
using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.Extensions;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services.CandlesServices;

public class HistoryCandlesService : 
    INotificationHandler<UserLoginEvent>,
    IRequestHandler<ReplyHistoryEvent>
{
    private readonly ILogger<HistoryCandlesService> _logger;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICoinsService _coinsService;
    private BinanceClient _client;
    
    private readonly Dictionary<PairIntervalKey, List<Quote>> _quotes = new();
    private readonly Dictionary<PairIntervalKey, Dictionary<IndicatorEnum, List<decimal?>>> _indicators = new();
    private readonly IMediator _mediator;

    public HistoryCandlesService(ILogger<HistoryCandlesService> logger, ICurrentUserProvider currentUserProvider,
        ICoinsService coinsService, 
        IMediator mediator)
    {
        _logger = logger;
        _currentUserProvider = currentUserProvider;
        _coinsService = coinsService;
        _mediator = mediator;
    }
    
    public async Task Handle(UserLoginEvent notification, CancellationToken cancellationToken)
    {
        _client ??= _currentUserProvider.GetClient();
    }
    
    public async Task<Unit> Handle(ReplyHistoryEvent request, CancellationToken cancellationToken)
    {
        var pairs = request.Pairs ?? await _coinsService.GetMostCapitalisedPairs(cancellationToken);
        var intervals = request.Intervals ?? Constants.DefaultIntervals;

        foreach (var pair in pairs)
        {
            foreach (var interval in intervals)
            {
                var candlesKey = new PairIntervalKey(pair, interval);
                _quotes[candlesKey] = (await _client.SpotApi.ExchangeData.GetKlinesAsync(pair.ToString(), interval, 
                    startTime: DateTime.UtcNow.Subtract(request.HistorySpan), ct: cancellationToken))
                    .Data.Select(x => x.ToQuote()).ToList();
                SetIndicators(candlesKey);
            }
        }

        var totalCount = _quotes[new PairIntervalKey(pairs.First(), Constants.DefaultIntervals.First())].Count;

        for (int c = 0; c < totalCount; c++)
            foreach (var pair in pairs)
                foreach (var interval in Constants.DefaultIntervals)
                {
                    var candlesKey = new PairIntervalKey(pair, interval);
                    var indicators = _indicators[candlesKey].ToDictionary(x => x.Key, x => x.Value[c]);

                    var quote = _quotes[candlesKey][c];
                    await _mediator.Send(new KlineUpdateEvent(pair, quote, indicators, true), cancellationToken);
                }
        return Unit.Value;
    }
        
    private void SetIndicators(PairIntervalKey pairIntervalKey)
    {
        var quotes = _quotes[pairIntervalKey];
        var ind = _indicators[pairIntervalKey];
        ind[IndicatorEnum.SMA_20] = quotes.GetSma(20).Select(x => x.Sma).Cast<decimal?>().ToList();
        ind[IndicatorEnum.SMA_50] = quotes.GetSma(50).Select(x => x.Sma).Cast<decimal?>().ToList();
        ind[IndicatorEnum.SMA_100] = quotes.GetSma(100).Select(x => x.Sma).Cast<decimal?>().ToList();
        ind[IndicatorEnum.SMA_200] = quotes.GetSma(200).Select(x => x.Sma).Cast<decimal?>().ToList();
        ind[IndicatorEnum.EMA_20] = quotes.GetEma(20).Select(x => x.Ema).Cast<decimal?>().ToList();
        ind[IndicatorEnum.EMA_50] = quotes.GetEma(50).Select(x => x.Ema).Cast<decimal?>().ToList();
        ind[IndicatorEnum.EMA_100] = quotes.GetEma(100).Select(x => x.Ema).Cast<decimal?>().ToList();
        ind[IndicatorEnum.EMA_200] = quotes.GetEma(200).Select(x => x.Ema).Cast<decimal?>().ToList();
        ind[IndicatorEnum.MACD_H] = quotes.GetMacd().Select(x => x.Histogram).Cast<decimal?>().ToList();
        ind[IndicatorEnum.RSI] = quotes.GetRsi().Select(x => x.Rsi).Cast<decimal?>().ToList();
        ind[IndicatorEnum.PARABOLIC_SAR] = quotes.GetParabolicSar().Select(x => x.Sar).Cast<decimal?>().ToList();
        ind[IndicatorEnum.BOLLINGER_UPPER] = quotes.GetBollingerBands().Select(x => x.UpperBand).Cast<decimal?>().ToList();
        ind[IndicatorEnum.BOLLINGER_LOWER] = quotes.GetBollingerBands().Select(x => x.LowerBand).Cast<decimal?>().ToList();        
    }
}
