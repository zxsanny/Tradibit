using Refit;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Dashboard;
using Tradibit.SharedUI.DTO.Primitives;
using Tradibit.SharedUI.DTO.Scenarios;

namespace Tradibit.SharedUI.Interfaces.API;

public interface IStrategiesApi
{
    [Get("/strategies/dashboard")]
    Task<Response<UserDashboard>> GetUserDashboard(GetCurrentUserDashboardRequest request);
    
    [Get("/strategies/available")]
    Task<Response<List<IdName>>> GetAvailableStrategies(GetAvailableStrategiesRequest request);

    [Post("/strategies/backtest")]
    Task<Response> BackTestStrategy(StartBackTestStrategyEvent e);
    
    [Post("/strategies/user")]
    Task<Response> AddStrategyToUser(AddStrategyToUserRequest request);
    
    [Delete("/strategies/user")]
    Task<Response> RemoveStrategyFromUser(RemoveStrategyFromUserRequest request);
}