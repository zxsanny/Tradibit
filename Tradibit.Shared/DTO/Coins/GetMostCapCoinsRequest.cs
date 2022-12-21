using MediatR;

namespace Tradibit.Common.DTO.Coins;

public class GetMostCapCoinsRequest : IRequest<List<Pair>>
{
    public Guid UserId { get; set; }

    public GetMostCapCoinsRequest(Guid userId)
    {
        UserId = userId;
    }
}