using MediatR;
using Tradibit.Shared.Entities;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Dashboard;
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
    
    public async Task<Response<List<PagedResponse<ScenarioDto>>>> GetScenarios(GetScenariosRequest request) =>
        await Send(request);

    public async Task<Response> CreateScenario(ScenarioCreatedEvent command) =>
        await Send(command);
    
    public async Task<Response> TestScenarioHistory(StartHistoryTestScenarioEvent query) =>
        await Send(query);

    public async Task<Response> StartScenario(StartScenarioEvent query) =>
        await Send(query);
}
