using MediatR;
using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.DTO.Scenarios;

public class ReplyHistoryEvent : IRequest<Unit>
{
    public Guid BackTestRunId { get; set; }
    public TimeSpan HistorySpan { get; set; }
    public IEnumerable<PairInterval> PairIntervals { get; set; }
    
    public ReplyHistoryEvent(Guid backTestRunId, TimeSpan historySpan, IEnumerable<PairInterval> pairIntervals)
    {
        BackTestRunId = backTestRunId;
        HistorySpan = historySpan;
        PairIntervals = pairIntervals;
    }
}