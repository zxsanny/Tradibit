using MediatR;
using Tradibit.Common.DTO;

namespace Tradibit.SharedUI.DTO.Scenarios;

public class GetScenariosQuery : PagedQuery, IRequest<List<PagedResponse<Scenario>>>
{
    public Guid UserId { get; set; }
}