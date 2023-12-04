using Binance.Net.Clients;
using Skender.Stock.Indicators;
using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Primitives;
using Quote = Tradibit.Shared.DTO.Primitives.Quote;
using SkenderQuote = Skender.Stock.Indicators.Quote;

namespace Tradibit.Api.Services;

public class QuoteIndicators
{
    private readonly Dictionary<PairInterval, List<Quote>> _quotes = new();
    private readonly Dictionary<PairInterval, Dictionary<IndicatorEnum, List<decimal?>>> _indicators = new();

    public async Task AddPairInterval(BinanceClient binanceClient, PairInterval pairInterval, TimeSpan? timeSpan, CancellationToken cancellationToken = default)
    {
        var kLinesResult = await binanceClient.SpotApi.ExchangeData.GetKlinesAsync(
            symbol: pairInterval.Pair.ToString(),
            interval: pairInterval.Interval.ToKlineInterval(),
            startTime: timeSpan.HasValue ? DateTime.UtcNow.Subtract(timeSpan.Value) : null,
            ct: cancellationToken);
        _quotes[pairInterval] = kLinesResult.Data.Select(x => x.ToQuote()).ToList();

        SetIndicators(pairInterval);
    }
    
    public QuoteIndicator Update(Quote quote, PairInterval pairInterval)
    {
        var quotes = _quotes[pairInterval];
        
        if (quotes[^1].Date == quote.Date) 
            quotes[^1].Update(quote);
        else
            quotes.Add(quote);
        SetIndicators(pairInterval);

        var indicator = _indicators[pairInterval].ToDictionary(x => x.Key, x => x.Value.LastOrDefault());
        return new QuoteIndicator(quote, indicator);
    }

    public List<Dictionary<PairInterval, QuoteIndicator>> GetHistory(IEnumerable<PairInterval> pairIntervals)
    {
        var pairQuotes = _quotes.Where(q => pairIntervals.Contains(q.Key)).ToList();
        return pairQuotes.FirstOrDefault().Value.Select((_, i) => pairIntervals.ToDictionary(k => k, pi =>
        {
            var q = _quotes[pi][i];
            var indicators = _indicators[pi].ToDictionary(x => x.Key, x => x.Value[i]);
            return new QuoteIndicator(q, indicators);
        })).ToList();
    }

    public Quote? LastQuote(PairInterval pairInterval) => _quotes[pairInterval].LastOrDefault(); 
    
    private void SetIndicators(PairInterval pairInterval)
    {
        var quotes = _quotes[pairInterval].Select(x => x.ToSkenderQuote()).ToList();
        var ind = _indicators[pairInterval];
        
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
}
