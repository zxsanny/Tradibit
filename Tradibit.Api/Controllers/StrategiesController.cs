using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Dashboard;
using Tradibit.SharedUI.DTO.Primitives;
using Tradibit.SharedUI.DTO.Scenarios;
using Tradibit.SharedUI.Interfaces.API;

namespace Tradibit.Api.Controllers;

public class StrategiesController : TradibitBaseController, IStrategiesApi
{
    public StrategiesController(IMediator mediator, ILogger<StrategiesController> logger) : base(mediator, logger)
    {
    }

    [HttpGet("/strategies/dashboard")]
    public async Task<UserDashboard> GetUserDashboard([FromQuery]GetCurrentUserDashboardRequest request) =>
        await Send(request);

    [HttpGet("/strategies/available")]
    public async Task<List<IdName>> GetAvailableStrategies([FromQuery]GetAvailableStrategiesRequest request) =>
        await Send(request);
    
    [HttpPost("/strategies/backtest")]
    public async Task BackTestStrategy(StartBackTestStrategyEvent e) =>
        await Send(e);
    
    [HttpPost("/strategies/user")]
    public async Task AddStrategyToUser(AddStrategyToUserRequest request) =>
        await Send(request);

    [HttpDelete("/strategies/user")]
    public async Task RemoveStrategyFromUser(RemoveStrategyFromUserRequest request) =>
        await Send(request);
}