using MediatR;

namespace Tradibit.SharedUI.DTO.Scenarios;

public class AddStrategyToUserRequest : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid StrategyId { get; set; }
}

public class RemoveStrategyFromUserRequest : AddStrategyToUserRequest {}