using MediatR;
using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.DTO.UserBroker;

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
