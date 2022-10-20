using Binance.Net.Enums;
using Binance.Net.Interfaces;
using CryptoExchange.Net.Sockets;
using Skender.Stock.Indicators;
using Tradibit.Api.Services.Binance;
using Tradibit.Common;
using Tradibit.Common.DTO;
using Tradibit.Common.Extensions;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services.Candles;

public class RealtimeCandlesService : BaseBinanceStreamService, ICandlesService
{
    private readonly ICoinsService _coinsService;
        
    private readonly Dictionary<PairIntervalKey, List<Quote>> _quotes = new();
    private readonly Dictionary<PairIntervalKey, Dictionary<IndicatorEnum, List<decimal?>>> _indicators = new();
    
    private Func<Pair, Quote, Dictionary<IndicatorEnum, decimal?>, Task> _handler;
    
    public RealtimeCandlesService(ILogger<RealtimeCandlesService> logger, ICurrentUserProvider currentUserProvider, ICoinsService coinsService)
        :base(logger, currentUserProvider)
    {
        _coinsService = coinsService;
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

        _handler?.Invoke(pair, quotes[^1], indicators);
    }
    
    public async Task SubscribeKlineHandler(Func<Pair, Quote, Dictionary<IndicatorEnum, decimal?>, Task> handler)
    {
        _handler = handler;
    }

    protected override async Task<List<int>> LoginHandle(CancellationToken cancellationToken = default)
    {
        var pairs = await _coinsService.GetMostCapitalisedPairs(cancellationToken);
        foreach (var pair in pairs)
        {
            foreach (var interval in Constants.DefaultIntervals)
            {
                var candlesKey = new PairIntervalKey(pair, interval);
                _quotes[candlesKey] = (await Client.SpotApi.ExchangeData.GetKlinesAsync(pair.ToString(), interval, ct: cancellationToken))
                    .Data.Select(x => x.ToQuote()).ToList();
                SetIndicators(candlesKey);
            }
        }

        var res = await SocketClient.SpotStreams.SubscribeToKlineUpdatesAsync(pairs.Select(x => x.ToString()), Constants.DefaultIntervals, OnMessage, cancellationToken);
        return new List<int> { res.Data.Id };
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
