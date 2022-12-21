using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Dashboard;

namespace Tradibit.Common.Interfaces.API;

public interface IScenariosApi
{
    Task<Response<UserDashboard>> GetUserDashboard(GetCurrentUserDashboardRequest request);
}