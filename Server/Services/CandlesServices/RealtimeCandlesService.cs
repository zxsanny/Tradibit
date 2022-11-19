using Binance.Net.Clients;
using Binance.Net.Interfaces;
using CryptoExchange.Net.Sockets;
using MediatR;
using Skender.Stock.Indicators;
using Tradibit.Api.Scenarios;
using Tradibit.Common;
using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.Extensions;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services.CandlesServices;

public class RealtimeCandlesService :
    INotificationHandler<UserLoginEvent>,
    INotificationHandler<UserLogoutEvent>
{
    private BinanceClient _client;
    private BinanceSocketClient _socketClient;
    private int? _subscription;
    private readonly Dictionary<PairIntervalKey, List<Quote>> _quotes = new();
    private readonly Dictionary<PairIntervalKey, Dictionary<IndicatorEnum, List<decimal?>>> _indicators = new();
    
    private readonly ILogger<RealtimeCandlesService> _logger;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICoinsService _coinsService;
    private readonly IMediator _mediator;

    public RealtimeCandlesService(ILogger<RealtimeCandlesService> logger, 
        ICurrentUserProvider currentUserProvider, 
        ICoinsService coinsService,
        IMediator mediator)
    {
        _logger = logger;
        _currentUserProvider = currentUserProvider;
        _coinsService = coinsService;
        _mediator = mediator;
    }
    
    private void OnMessage(DataEvent<IBinanceStreamKlineData> msg)
    {
        var quote = msg.Data.Data.ToQuote();
        var pair = Pair.ParseOrDefault(msg.Data.Symbol);
        var candlesKey = new PairIntervalKey(pair, msg.Data.Data.Interval);
        var quotes = _quotes[candlesKey];

        if (quotes[^1].Date == quote.Date) 
            quotes[^1].Update(quote);
        else
            quotes.Add(quote);
        SetIndicators(candlesKey);

        var indicators = _indicators[candlesKey].ToDictionary(x => x.Key, x => x.Value.LastOrDefault());

        _mediator.Send(new KlineUpdateEvent(pair, quote, indicators));
    }

    
    public async Task Handle(UserLoginEvent notification, CancellationToken cancellationToken)
    {
        _client ??= _currentUserProvider.GetClient();
        _socketClient ??= _currentUserProvider.GetSocketClient();
        if (_quotes.Any())
            return;
        
        var pairs = await _coinsService.GetMostCapitalisedPairs(cancellationToken);
        foreach (var pair in pairs)
        {
            foreach (var interval in Constants.DefaultIntervals)
            {
                var candlesKey = new PairIntervalKey(pair, interval);
                _quotes[candlesKey] = (await _client.SpotApi.ExchangeData.GetKlinesAsync(pair.ToString(), interval, ct: cancellationToken))
                    .Data.Select(x => x.ToQuote()).ToList();
                SetIndicators(candlesKey);
            }
        }
        
        var res = await _socketClient.SpotStreams.SubscribeToKlineUpdatesAsync(pairs.Select(x => x.ToString()), Constants.DefaultIntervals, OnMessage,
            cancellationToken);
        _subscription = res.Data.Id;
    }

    public async Task Handle(UserLogoutEvent notification, CancellationToken cancellationToken)
    {
        if (_subscription.HasValue) 
            await _socketClient.UnsubscribeAsync(_subscription.Value);
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
