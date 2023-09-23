using MediatR;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO.UserBroker;

public abstract class BaseOrderEvent : IRequest<decimal>
{
    public Guid UserId { get; set; }
    public Pair? Pair { get; set; }
    public decimal Amount { get; set; }

    protected BaseOrderEvent(Guid userId, Pair? pair, decimal amount)
    {
        UserId = userId;
        Pair = pair;
        Amount = amount;
    }
}

public class BuyEvent : BaseOrderEvent
{
    public BuyEvent(Guid userId, Pair pair, decimal amount) : base(userId, pair, amount)
    {
    }
}
public class SellEvent: BaseOrderEvent
{
    public SellEvent(Guid userId, Pair? pair, decimal amount) : base(userId, pair, amount)
    {
    }
}
