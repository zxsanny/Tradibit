using MediatR;
using Tradibit.Common.DTO;
using Tradibit.SharedUI.Primitives;

namespace Tradibit.SharedUI.DTO.Coins;

public class GetMostCapCoinsRequest : IRequest<List<Pair>>
{
    public Guid UserId { get; set; }

    public GetMostCapCoinsRequest(Guid userId)
    {
        UserId = userId;
    }
}