using Refit;
using Tradibit.SharedUI.DTO.Dashboard;
using Tradibit.SharedUI.DTO.Primitives;
using Tradibit.SharedUI.DTO.Scenarios;

namespace Tradibit.SharedUI.Interfaces.API;

public interface IStrategiesApi
{
    [Get("/strategies/dashboard")]
    Task<UserDashboard> GetUserDashboard(GetCurrentUserDashboardRequest request);
    
    [Get("/strategies/available")]
    Task<List<IdName>> GetAvailableStrategies(GetAvailableStrategiesRequest request);

    [Post("/strategies/backtest")]
    Task BackTestStrategy(StartBackTestStrategyEvent e);
    
    [Post("/strategies/user")]
    Task AddStrategyToUser(AddStrategyToUserRequest request);
    
    [Delete("/strategies/user")]
    Task RemoveStrategyFromUser(RemoveStrategyFromUserRequest request);
}