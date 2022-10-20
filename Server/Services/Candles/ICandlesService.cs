using Skender.Stock.Indicators;
using Tradibit.Common.DTO;

namespace Tradibit.Api.Services.Candles;

public interface ICandlesService
{
    Task SubscribeKlineHandler(Func<Pair, Quote, Dictionary<IndicatorEnum, decimal?>, Task> handler);
}

public enum CandlesResolverEnum
{
    Realtime,
    History
}

public delegate ICandlesService CandlesProviderResolver (CandlesResolverEnum candlesResolverEnum);
