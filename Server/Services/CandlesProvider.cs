using AutoMapper;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using CryptoExchange.Net.Sockets;
using MediatR;
using Skender.Stock.Indicators;
using Tradibit.SharedUI;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Coins;
using Tradibit.SharedUI.DTO.Primitives;
using Tradibit.SharedUI.DTO.Scenarios;
using Tradibit.SharedUI.DTO.Users;
using Tradibit.SharedUI.Interfaces;
using Quote = Tradibit.SharedUI.DTO.Primitives.Quote;
using SkenderQuote = Skender.Stock.Indicators.Quote;

namespace Tradibit.Api.Services;

public class CandlesProvider : ICandlesProvider,
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
    private readonly IClientHolder _clientHolder;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private int _subscription;

    public CandlesProvider(ILogger<CandlesProvider> logger, IClientHolder clientHolder, IMediator mediator, IMapper mapper)
    {
        _logger = logger;
        _clientHolder = clientHolder;
        _mediator = mediator;
        _mapper = mapper;
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

        var pairs = await _mediator.Send(new GetMostCapCoinsRequest(userLoginEvent.UserId), cancellationToken);
        var intervals = Constants.DefaultIntervals.Select(interval => 
            (
                Interval: interval,
                BinanceInterval: _mapper.Map<KlineInterval>(interval))
            ).ToList();
        foreach (var pair in pairs)
        {
            foreach (var interval in intervals)
            {
                var candlesKey = new PairIntervalKey(pair, interval.Interval);
                Quotes[candlesKey] = (await _client.SpotApi.ExchangeData.GetKlinesAsync(pair.ToString(), interval.BinanceInterval, ct: cancellationToken))
                    .Data.Select(x => _mapper.Map<Quote>(x)).ToList();
                SetIndicators(candlesKey);
            }
        }
        
        var res = await _socketClient.SpotStreams.SubscribeToKlineUpdatesAsync(pairs.Select(x => x.ToString()), 
            intervals.Select(x => x.BinanceInterval), OnMessage, cancellationToken);
        _subscription = res.Data.Id;
    }
    
    private void OnMessage(DataEvent<IBinanceStreamKlineData> msg)
    {
        var quote = _mapper.Map<Quote>(msg.Data.Data);
        var pair = Pair.ParseOrDefault(msg.Data.Symbol);
        var candlesKey = new PairIntervalKey(pair, _mapper.Map<Interval>(msg.Data.Data.Interval));
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
        var pairs = e.Pairs ?? await _mediator.Send(new GetMostCapCoinsRequest(e.UserId), cancellationToken);
        var intervals = e.Intervals ?? Constants.DefaultIntervals;

        foreach (var pair in pairs)
        {
            foreach (var interval in intervals)
            {
                var candlesKey = new PairIntervalKey(pair, interval);
                _historyQuotes[candlesKey] = (await _client.SpotApi.ExchangeData.GetKlinesAsync(pair.ToString(), _mapper.Map<KlineInterval>(interval), 
                        startTime: DateTime.UtcNow.Subtract(e.HistorySpan), ct: cancellationToken))
                    .Data.Select(kline => _mapper.Map<Quote>(kline)).ToList();
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
        ind[IndicatorEnum.SMA_20] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetSma(20).Select(x => (decimal?)x.Sma).ToList();
        ind[IndicatorEnum.SMA_50] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetSma(50).Select(x => (decimal?)x.Sma).ToList();
        ind[IndicatorEnum.SMA_100] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetSma(100).Select(x =>(decimal?) x.Sma).ToList();
        ind[IndicatorEnum.SMA_200] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetSma(200).Select(x =>(decimal?) x.Sma).ToList();
        ind[IndicatorEnum.EMA_20] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetEma(20).Select(x =>(decimal?) x.Ema).ToList();
        ind[IndicatorEnum.EMA_50] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetEma(50).Select(x =>(decimal?) x.Ema).ToList();
        ind[IndicatorEnum.EMA_100] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetEma(100).Select(x =>(decimal?) x.Ema).ToList();
        ind[IndicatorEnum.EMA_200] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetEma(200).Select(x =>(decimal?) x.Ema).ToList();
        ind[IndicatorEnum.MACD_H] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetMacd().Select(x => (decimal?)x.Histogram).ToList();
        ind[IndicatorEnum.RSI] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetRsi().Select(x => (decimal?)x.Rsi).ToList();
        ind[IndicatorEnum.PARABOLIC_SAR] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetParabolicSar().Select(x => (decimal?)x.Sar).ToList();
        ind[IndicatorEnum.BOLLINGER_UPPER] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetBollingerBands().Select(x => (decimal?)x.UpperBand).ToList();
        ind[IndicatorEnum.BOLLINGER_LOWER] = quotes.Select(q => _mapper.Map<SkenderQuote>(q)).GetBollingerBands().Select(x => (decimal?)x.LowerBand).ToList();        
    }

    public decimal BtcValue => 
        Quotes[new PairIntervalKey(new Pair(Currency.BTC, Currency.USDT), Interval.I_15_MIN)].LastOrDefault()?.Close ?? 0;
}