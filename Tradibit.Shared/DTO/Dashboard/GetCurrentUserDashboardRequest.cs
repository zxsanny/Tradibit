using MediatR;

namespace Tradibit.SharedUI.DTO.Dashboard;

public class GetCurrentUserDashboardRequest : IRequest<UserDashboard> { }