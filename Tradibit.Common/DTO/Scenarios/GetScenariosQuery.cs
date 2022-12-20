using MediatR;
using Tradibit.Common.DTO.Queries;
using Tradibit.Common.Entities;

namespace Tradibit.Common.DTO.Scenarios;

public class GetScenariosQuery : PagedQuery, IRequest<List<PagedResponse<Scenario>>>
{
    public Guid UserId { get; set; }
}