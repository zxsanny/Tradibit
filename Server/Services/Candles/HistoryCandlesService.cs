using Skender.Stock.Indicators;
using Tradibit.Api.Services.Binance;
using Tradibit.Common;
using Tradibit.Common.DTO;
using Tradibit.Common.Extensions;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services.Candles;

public class HistoryCandlesService : BaseBinanceService, ICandlesService
{
    private Func<Pair,Quote,Dictionary<IndicatorEnum, decimal?>, Task> _handler;
    private readonly ICoinsService _coinsService;
    
    private readonly Dictionary<PairIntervalKey, List<Quote>> _quotes = new();
    private readonly Dictionary<PairIntervalKey, Dictionary<IndicatorEnum, List<decimal?>>> _indicators = new();
    
    public HistoryCandlesService(ILogger<HistoryCandlesService> logger, ICurrentUserProvider currentUserProvider, ICoinsService coinsService)
        : base(logger, currentUserProvider)
    {
        _coinsService = coinsService;
    }

    public async Task SubscribeKlineHandler(Func<Pair, Quote, Dictionary<IndicatorEnum, decimal?>, Task> handler)
    {
        _handler = handler;
    }

    
    public async Task StartProcessHistory(TimeSpan historySpan, CancellationToken cancellationToken = default)
    {
        var pairs = await _coinsService.GetMostCapitalisedPairs(cancellationToken);
        foreach (var pair in pairs)
        {
            foreach (var interval in Constants.DefaultIntervals)
            {
                var candlesKey = new PairIntervalKey(pair, interval);
                _quotes[candlesKey] = (await Client.SpotApi.ExchangeData.GetKlinesAsync(pair.ToString(), interval, 
                    startTime: DateTime.UtcNow.Subtract(historySpan), ct: cancellationToken))
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
                    _handler?.Invoke(pair, _quotes[candlesKey][c], indicators);
                }
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