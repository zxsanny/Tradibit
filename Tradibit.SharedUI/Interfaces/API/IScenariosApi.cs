using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Dashboard;

namespace Tradibit.SharedUI.Interfaces.API;

public interface IScenariosApi
{
    Task<Response<UserDashboard>> GetUserDashboard(GetCurrentUserDashboardRequest request);
}