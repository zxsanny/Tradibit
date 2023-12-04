using MediatR;

namespace Tradibit.Shared.DTO.Scenarios;

public class RemoveScenarioEvent : IRequest<Unit>
{
    public Guid? StrategyId { get; set; }
    public Guid? ScenarioId { get; set; }
}