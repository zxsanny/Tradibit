using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tradibit.Shared.DTO.Dashboard;
using Tradibit.Shared.DTO.Primitives;
using Tradibit.Shared.DTO.Scenarios;

namespace Tradibit.Api.Controllers;

public class ScenariosController : TradibitBaseController
{
    public ScenariosController(IMediator mediator, ILogger<ScenariosController> logger) : base(mediator, logger)
    {
    }

    [HttpGet("dashboard")]
    public async Task<UserDashboard> GetUserDashboard([FromQuery]GetCurrentUserDashboardRequest request) =>
        await Send(request);

    [HttpGet("strategies")]
    public async Task<List<IdName>> GetAvailableStrategies([FromQuery]GetAvailableStrategiesRequest request) =>
        await Send(request);
    
    [HttpPost]
    public async Task AddScenarios(AddScenariosEvent ev) =>
        await Send(ev);

    [HttpDelete]
    public async Task RemoveScenarios(RemoveScenarioEvent ev) =>
        await Send(ev);
    
    [HttpPost("backtest")]
    public async Task BackTestStrategy(RunBackTestEvent ev) =>
        await Send(ev);
}