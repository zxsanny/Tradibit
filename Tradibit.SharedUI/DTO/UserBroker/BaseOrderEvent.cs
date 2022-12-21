using MediatR;
using Tradibit.SharedUI.Primitives;

namespace Tradibit.Common.DTO.Events.UserBroker;

public abstract class BaseOrderEvent : IRequest<decimal>
{
    public Guid UserId { get; set; }
    public Pair Pair { get; set; }
    public decimal Amount { get; set; }
}

public class BuyEvent : BaseOrderEvent {}
public class SellEvent: BaseOrderEvent {}
