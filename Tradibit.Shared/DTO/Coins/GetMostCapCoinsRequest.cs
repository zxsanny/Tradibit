using MediatR;
using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.DTO.Coins;

public class GetMostCapCoinsRequest : IRequest<List<Pair>>
{ }