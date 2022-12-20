using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Dashboard;
using Tradibit.Common.DTO.Queries;
using Tradibit.Common.DTO.Scenarios;
using Tradibit.Common.Entities;

namespace Tradibit.Common.Interfaces.API;

public interface IScenariosApi
{
    Task<Response<UserDashboard>> GetUserDashboard(GetCurrentUserDashboardRequest request);
}