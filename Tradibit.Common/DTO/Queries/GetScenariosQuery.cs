using MediatR;
using Tradibit.Common.Entities;

namespace Tradibit.Common.DTO.Queries;

public class GetScenariosQuery : PagedQuery, IRequest<List<PagedResponse<Scenario>>>
{
    public Guid UserId { get; set; }
}