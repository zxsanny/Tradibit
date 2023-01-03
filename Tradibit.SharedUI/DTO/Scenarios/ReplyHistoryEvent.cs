using MediatR;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO.Scenarios;

public class ReplyHistoryEvent : IRequest<Unit>
{
    public Guid StrategyId { get; set; }
    public TimeSpan HistorySpan { get; set; }
    public List<Pair>? Pairs { get; set; }
    public List<Interval>? Intervals { get; set; }

    public ReplyHistoryEvent(TimeSpan historySpan, List<Pair> pairs, List<Interval> intervals)
    {
        HistorySpan = historySpan;
        Pairs = pairs;
        Intervals = intervals;
    }
}