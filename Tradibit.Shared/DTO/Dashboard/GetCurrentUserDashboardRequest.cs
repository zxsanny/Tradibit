using MediatR;

namespace Tradibit.Shared.DTO.Dashboard;

public class GetCurrentUserDashboardRequest : IRequest<UserDashboard> { }