using MediatR;
using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.DTO.Scenarios;

public class RunBackTestEvent : IRequest<Unit>
{
    public Guid            BackTestRunId { get; set; }
    public Guid            StrategyId { get; set; }
    
    public decimal         Deposit { get; set; }
    public int             MaxActiveTrades { get; set; }
    
    public List<Pair>?     Pairs      { get; set; }
    public List<Interval>? Intervals  { get; set; }
    
    public TimeSpan        HistorySpan { get; set; }
}