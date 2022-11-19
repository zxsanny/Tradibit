using Binance.Net.Enums;
using MediatR;

namespace Tradibit.Common.DTO.Events;

public class StartScenarioEvent : BaseScenarioEvent 
{
}

public class StartHistoryTestScenarioEvent : BaseScenarioEvent
{
    public decimal Deposit { get; set; }
    public TimeSpan HistorySpan { get; set; } 
    public List<Pair> Pairs { get; set; }
    public List<KlineInterval> Intervals { get; set; }
}

public class ReplyHistoryEvent : IRequest<Unit>
{
    public TimeSpan HistorySpan { get; set; } 
    public List<Pair> Pairs { get; set; }
    public List<KlineInterval> Intervals { get; set; }

    public ReplyHistoryEvent(TimeSpan historySpan, List<Pair> pairs, List<KlineInterval> intervals)
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
}