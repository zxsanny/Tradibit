using MediatR;

namespace Tradibit.Common.DTO.Events.Coins;

public class GetMostCapCoinsEvent : IRequest<List<Pair>>
{
    public Guid UserId { get; set; }

    public GetMostCapCoinsEvent(Guid userId)
    {
        UserId = userId;
    }
}