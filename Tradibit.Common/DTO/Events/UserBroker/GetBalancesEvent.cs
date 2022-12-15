using MediatR;

namespace Tradibit.Common.DTO.Events.UserBroker;

public class GetBalancesEvent : IRequest<Dictionary<Currency, decimal>>
{
    public Guid UserId { get; set; }
    public string Asset { get; set; }

    public GetBalancesEvent(Guid userId, string asset)
    {
        UserId = userId;
        Asset = asset;
    }
}
