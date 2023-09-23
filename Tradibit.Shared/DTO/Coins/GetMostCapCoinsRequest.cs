using MediatR;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO.Coins;

public class GetMostCapCoinsRequest : IRequest<List<Pair>>
{
    
}