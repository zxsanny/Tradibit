using MediatR;
using Tradibit.SharedUI.DTO.Dashboard;

namespace Tradibit.SharedUI.DTO.Scenarios;

public class GetScenariosRequest : PagedRequest, IRequest<List<PagedResponse<ScenarioDto>>>
{
    public Guid UserId { get; set; }
}