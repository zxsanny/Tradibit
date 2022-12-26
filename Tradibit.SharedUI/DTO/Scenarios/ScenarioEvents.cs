using MediatR;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO.Scenarios;

public class StartScenarioEvent : BaseScenarioEvent
{
}

public class StartHistoryTestScenarioEvent : BaseScenarioEvent
{
    public decimal Deposit { get; set; }
    public TimeSpan HistorySpan { get; set; }
    public List<Pair>? Pairs { get; set; }
    public List<Interval>? Intervals { get; set; }
}

public class ReplyHistoryEvent : BaseScenarioEvent
{
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

public class StopScenarioEvent : BaseScenarioEvent
{
    public bool IsHistory { get; set; }
}

public abstract class BaseScenarioEvent : IRequest<Unit>
{
    public Guid ScenarioId { get; set; }
    public Guid UserId { get; set; }
}