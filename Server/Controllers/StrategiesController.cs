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
    public StrategiesController(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    [HttpGet("/strategies/dashboard")]
    public async Task<Response<UserDashboard>> GetUserDashboard(GetCurrentUserDashboardRequest request) =>
        await Send(request);

    [HttpGet("/strategies/available")]
    public async Task<Response<List<IdName>>> GetAvailableStrategies(GetAvailableStrategiesRequest request) =>
        await Send(request);
    
    [HttpPost("/strategies/backtest")]
    public async Task<Response> BackTestStrategy(StartBackTestStrategyEvent e) =>
        await Send(e);
    
    [HttpPost("/strategies/user")]
    public async Task<Response> AddStrategyToUser(AddStrategyToUserRequest request) =>
        await Send(request);

    [HttpDelete("/strategies/user")]
    public async Task<Response> RemoveStrategyFromUser(RemoveStrategyFromUserRequest request) =>
        await Send(request);
}