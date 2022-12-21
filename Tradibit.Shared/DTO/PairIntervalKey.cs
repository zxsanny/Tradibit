using Binance.Net.Enums;

namespace Tradibit.Common.DTO;

public class PairIntervalKey
{
    private Pair Pair { get; set; }
    private KlineInterval Interval { get; set; }

    public PairIntervalKey(Pair pair, KlineInterval interval)
    {
        Pair = pair;
        Interval = interval;
    }
}