namespace Tradibit.SharedUI.DTO.Primitives;

public class PairIntervalKey
{
    public Pair Pair { get; set; }
    public Interval Interval { get; set; }

    public PairIntervalKey(Pair pair, Interval interval)
    {
        Pair = pair;
        Interval = interval;
    }
}