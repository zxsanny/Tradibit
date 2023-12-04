namespace Tradibit.Shared.DTO.Primitives;

public class PairInterval : IEquatable<PairInterval>
{
    public Pair Pair { get; }
    public Interval Interval { get; }

    public PairInterval(Pair pair, Interval interval)
    {
        Pair = pair;
        Interval = interval;
    }

    public bool Equals(PairInterval? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Pair.Equals(other.Pair) && Interval == other.Interval;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        
        return Equals((PairInterval)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Pair, (int?)Interval);
    }
    
    public static bool operator ==(PairInterval? pairInterval1, PairInterval? pairInterval2)
    {
        if (pairInterval1 is null)
            return pairInterval2 is null;
        
        return pairInterval1.Equals(pairInterval2);
    }

    public static bool operator !=(PairInterval pairInterval1, PairInterval pairInterval2) =>
        !(pairInterval1 == pairInterval2);
}

public static class PairIntervalExtensions
{
    public static IEnumerable<PairInterval> ToPainIntervals(this IEnumerable<Pair> pairs, IEnumerable<Interval> intervals) => 
        pairs.SelectMany(_ => intervals, (pair, interval) => new PairInterval(pair, interval)).ToList();
}