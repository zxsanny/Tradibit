namespace Tradibit.SharedUI.DTO.Primitives;

public class PairIntervalKey
{
    private Pair Pair { get; set; }
    private Interval Interval { get; set; }

    public PairIntervalKey(Pair pair, Interval interval)
    {
        Pair = pair;
        Interval = interval;
    }
}