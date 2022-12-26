using MediatR;

namespace Tradibit.SharedUI.DTO.Users;

public class UserLogoutEvent : INotification
{
    public Guid UserId { get; set; }
}