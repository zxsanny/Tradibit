using MediatR;

namespace Tradibit.Common.DTO.Events.Scenarios;

public class ScenarioCreatedEvent : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid StrategyId { get; set; }
}