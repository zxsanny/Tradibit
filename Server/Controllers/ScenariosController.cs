using MediatR;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Dashboard;
using Tradibit.SharedUI.DTO.Primitives;
using Tradibit.SharedUI.DTO.Scenarios;
using Tradibit.SharedUI.Interfaces.API;

namespace Tradibit.Api.Controllers;

public class ScenariosController : TradibitBaseController, IScenariosApi
{
    public ScenariosController(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    public async Task<Response<UserDashboard>> GetUserDashboard(GetCurrentUserDashboardRequest request) =>
        await Send(request);

    public async Task<Response<List<IdName>>> GetAvailableStrategies(GetAvailableStrategiesRequest request) =>
        await Send(request);
    
    public async Task<Response> AddStrategyToUser(AddStrategyToUserRequest request) =>
        await Send(request);

    public async Task<Response> RemoveStrategyFromUser(RemoveStrategyFromUserRequest request) =>
        await Send(request);
    
    public async Task<Response<List<PagedResponse<ScenarioDto>>>> GetScenarios(GetScenariosRequest request) =>
        await Send(request);
    
    public async Task<Response> TestScenarioHistory(StartBackTestStrategyEvent e) =>
        await Send(e);
}