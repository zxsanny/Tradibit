using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Dashboard;
using Tradibit.SharedUI.DTO.Primitives;
using Tradibit.SharedUI.DTO.Scenarios;

namespace Tradibit.SharedUI.Interfaces.API;

public interface IScenariosApi
{
    Task<Response<UserDashboard>> GetUserDashboard(GetCurrentUserDashboardRequest request);
    Task<Response<List<IdName>>> GetAvailableStrategies(GetAvailableStrategiesRequest request);
    Task<Response> AddStrategyToUser(AddStrategyToUserRequest request);
    Task<Response> RemoveStrategyFromUser(RemoveStrategyFromUserRequest request);

}