using MediatR;

namespace Tradibit.SharedUI.DTO.Scenarios;

public class ScenarioCreatedEvent : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid StrategyId { get; set; }
}