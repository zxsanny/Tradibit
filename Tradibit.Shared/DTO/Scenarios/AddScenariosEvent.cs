using MediatR;
using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.DTO.Scenarios;

public class AddScenariosEvent : IRequest<Unit>
{
    public Guid            StrategyId { get; set; }
    
    public Guid            UserId     { get; set; }
    
    public List<Pair>?     Pairs      { get; set; }
    public List<Interval>? Intervals  { get; set; }
}