using MediatR;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO.Scenarios;

public class GetAvailableStrategiesRequest : IRequest<List<IdName>>
{
}