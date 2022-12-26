using MediatR;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO.UserBroker;

public abstract class BaseOrderEvent : IRequest<decimal>
{
    public Guid UserId { get; set; }
    public Pair? Pair { get; set; }
    public decimal Amount { get; set; }
}

public class BuyEvent : BaseOrderEvent {}
public class SellEvent: BaseOrderEvent {}
