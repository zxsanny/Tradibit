using MediatR;

namespace Tradibit.SharedUI.DTO.Scenarios;

public class StrategyChangeStatusEvent : IRequest<Unit>
{
    public Guid StrategyId { get; set; }
    public bool IsEnabled { get; set; }
}