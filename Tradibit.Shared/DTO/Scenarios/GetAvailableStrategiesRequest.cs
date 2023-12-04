using MediatR;
using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.DTO.Scenarios;

public class GetAvailableStrategiesRequest : IRequest<List<IdName>>
{
}