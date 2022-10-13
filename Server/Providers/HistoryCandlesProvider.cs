using Skender.Stock.Indicators;
using Tradibit.Common.DTO;

namespace Tradibit.Api.Providers;

public class HistoryCandlesProvider : ICandlesProvider
{
    public Dictionary<PairIntervalKey, List<Quote>> Quotes { get; }
    public Dictionary<PairIntervalKey, Dictionary<IndicatorEnum, List<double?>>> Indicators { get; }
    
    public Task Subscribe(Action<Pair, Quote, Dictionary<IndicatorEnum, double?>> handler)
    {
        throw new NotImplementedException();
    }
}