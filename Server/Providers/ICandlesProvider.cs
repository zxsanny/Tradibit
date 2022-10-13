using Skender.Stock.Indicators;
using Tradibit.Common.DTO;

namespace Tradibit.Api.Providers;

public interface ICandlesProvider
{
    Dictionary<PairIntervalKey, List<Quote>> Quotes { get; }
    Dictionary<PairIntervalKey, Dictionary<IndicatorEnum, List<double?>>> Indicators { get; }
    
    Task Subscribe(Action<Pair, Quote, Dictionary<IndicatorEnum, double?>> handler);
}

public enum CandlesResolverEnum
{
    Realtime,
    History
}

public delegate ICandlesProvider? CandlesProviderResolver (CandlesResolverEnum candlesResolverEnum);
