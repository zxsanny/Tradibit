using MediatR;

namespace Tradibit.Common.DTO.Dashboard;

public class GetCurrentUserDashboardRequest : IRequest<UserDashboard> { }