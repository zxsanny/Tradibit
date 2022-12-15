using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using CryptoExchange.Net.Sockets;
using MediatR;
using Skender.Stock.Indicators;
using Tradibit.Common;
using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.DTO.Events.Coins;
using Tradibit.Common.DTO.Events.Scenarios;
using Tradibit.Common.Extensions;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services;

public class CandlesProvider : 
    ICandlesProvider,
    INotificationHandler<UserLoginEvent>,
    IRequestHandler<ReplyHistoryEvent>
{
    private BinanceClient _client;
    private BinanceSocketClient _socketClient;
    
    private static readonly Dictionary<PairIntervalKey, List<Quote>> Quotes = new();
    private static readonly Dictionary<PairIntervalKey, Dictionary<IndicatorEnum, List<decimal?>>> Indicators = new();
    
    private readonly Dictionary<PairIntervalKey, List<Quote>> _historyQuotes = new();
    private readonly Dictionary<PairIntervalKey, Dictionary<IndicatorEnum, List<decimal?>>> _historyIndicators = new();

    private readonly ILogger<CandlesProvider> _logger;
    private readonly ClientHolder _clientHolder;
    private readonly IMediator _mediator;
    private int _subscription;

    public CandlesProvider(ILogger<CandlesProvider> logger, ClientHolder clientHolder, IMediator mediator)
    {
        _logger = logger;
        _clientHolder = clientHolder;
        _mediator = mediator;
    }
    
    //On the very first login (any user), for the most cap coins for default intervals: 
    //1. Fill the _quotes and indicators
    //2. Subscribe on any KLine updates to constantly update _quotes and indicators
    public async Task Handle(UserLoginEvent userLoginEvent, CancellationToken cancellationToken)
    {
        _client ??= await _clientHolder.GetClient(userLoginEvent.UserId, cancellationToken);
        _socketClient ??= await _clientHolder.GetSocketClient(userLoginEvent.UserId, cancellationToken);
        if (Quotes.Any())
            return;

        var pairs = await _mediator.Send(new GetMostCapCoinsEvent(userLoginEvent.UserId), cancellationToken);
        foreach (var pair in pairs)
        {
            foreach (var interval in Constants.DefaultIntervals)
            {
                var candlesKey = new PairIntervalKey(pair, interval);
                Quotes[candlesKey] = (await _client.SpotApi.ExchangeData.GetKlinesAsync(pair.ToString(), interval, ct: cancellationToken))
                    .Data.Select(x => x.ToQuote()).ToList();
                SetIndicators(candlesKey);
            }
        }
        
        var res = await _socketClient.SpotStreams.SubscribeToKlineUpdatesAsync(pairs.Select(x => x.ToString()), Constants.DefaultIntervals, OnMessage,
            cancellationToken);
        _subscription = res.Data.Id;
    }
    
    private void OnMessage(DataEvent<IBinanceStreamKlineData> msg)
    {
        var quote = msg.Data.Data.ToQuote();
        var pair = Pair.ParseOrDefault(msg.Data.Symbol);
        var candlesKey = new PairIntervalKey(pair, msg.Data.Data.Interval);
        var quotes = Quotes[candlesKey];

        if (quotes[^1].Date == quote.Date) 
            quotes[^1].Update(quote);
        else
            quotes.Add(quote);
        SetIndicators(candlesKey);

        var indicators = Indicators[candlesKey].ToDictionary(x => x.Key, x => x.Value.LastOrDefault());
        
        _mediator.Send(new KlineUpdateEvent(pair, new QuoteIndicator(quote, indicators)));
    }

    
    public async Task<Unit> Handle(ReplyHistoryEvent e, CancellationToken cancellationToken)
    {
        var pairs = e.Pairs ?? await _mediator.Send(new GetMostCapCoinsEvent(e.UserId), cancellationToken);
        var intervals = e.Intervals ?? Constants.DefaultIntervals;

        foreach (var pair in pairs)
        {
            foreach (var interval in intervals)
            {
                var candlesKey = new PairIntervalKey(pair, interval);
                _historyQuotes[candlesKey] = (await _client.SpotApi.ExchangeData.GetKlinesAsync(pair.ToString(), interval, 
                        startTime: DateTime.UtcNow.Subtract(e.HistorySpan), ct: cancellationToken))
                    .Data.Select(x => x.ToQuote()).ToList();
                SetIndicators(candlesKey);
            }
        }

        var totalCount = _historyQuotes[new PairIntervalKey(pairs.First(), Constants.DefaultIntervals.First())].Count;

        for (int c = 0; c < totalCount; c++)
            foreach (var pair in pairs)
            {
                foreach (var interval in Constants.DefaultIntervals)
                {
                    var candlesKey = new PairIntervalKey(pair, interval);
                    var indicators = _historyIndicators[candlesKey].ToDictionary(x => x.Key, x => x.Value[c]);

                    var quote = _historyQuotes[candlesKey][c];
                    await _mediator.Send(new KlineHistoryUpdateEvent(e.ScenarioId, new KlineUpdateEvent(pair, new QuoteIndicator(quote, indicators))), cancellationToken);
                }   
            }
        return Unit.Value;
    }
    
    private void SetIndicators(PairIntervalKey pairIntervalKey)
    {
        var quotes = Quotes[pairIntervalKey];
        var ind = Indicators[pairIntervalKey];
        ind[IndicatorEnum.SMA_20] = quotes.GetSma(20).Select(x => (decimal?)x.Sma).ToList();
        ind[IndicatorEnum.SMA_50] = quotes.GetSma(50).Select(x => (decimal?)x.Sma).ToList();
        ind[IndicatorEnum.SMA_100] = quotes.GetSma(100).Select(x =>(decimal?) x.Sma).ToList();
        ind[IndicatorEnum.SMA_200] = quotes.GetSma(200).Select(x =>(decimal?) x.Sma).ToList();
        ind[IndicatorEnum.EMA_20] = quotes.GetEma(20).Select(x =>(decimal?) x.Ema).ToList();
        ind[IndicatorEnum.EMA_50] = quotes.GetEma(50).Select(x =>(decimal?) x.Ema).ToList();
        ind[IndicatorEnum.EMA_100] = quotes.GetEma(100).Select(x =>(decimal?) x.Ema).ToList();
        ind[IndicatorEnum.EMA_200] = quotes.GetEma(200).Select(x =>(decimal?) x.Ema).ToList();
        ind[IndicatorEnum.MACD_H] = quotes.GetMacd().Select(x => (decimal?)x.Histogram).ToList();
        ind[IndicatorEnum.RSI] = quotes.GetRsi().Select(x => (decimal?)x.Rsi).ToList();
        ind[IndicatorEnum.PARABOLIC_SAR] = quotes.GetParabolicSar().Select(x => (decimal?)x.Sar).ToList();
        ind[IndicatorEnum.BOLLINGER_UPPER] = quotes.GetBollingerBands().Select(x => (decimal?)x.UpperBand).ToList();
        ind[IndicatorEnum.BOLLINGER_LOWER] = quotes.GetBollingerBands().Select(x => (decimal?)x.LowerBand).ToList();        
    }

    public decimal BtcValue => 
        Quotes[new PairIntervalKey(new Pair(Currency.BTC, Currency.USDT), KlineInterval.FifteenMinutes)].LastOrDefault()?.Close ?? 0;
}