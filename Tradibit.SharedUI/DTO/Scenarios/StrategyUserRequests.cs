using MediatR;

namespace Tradibit.SharedUI.DTO.Scenarios;

public class AddStrategyToUserRequest : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid StrategyId { get; set; }

    public AddStrategyToUserRequest(Guid userId, Guid strategyId)
    {
        UserId = userId;
        StrategyId = strategyId;
    }
}

public class RemoveStrategyFromUserRequest : AddStrategyToUserRequest
{
    public RemoveStrategyFromUserRequest(Guid userId, Guid strategyId) : base(userId, strategyId)
    {
    }
}