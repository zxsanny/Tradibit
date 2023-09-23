using MediatR;

namespace Tradibit.SharedUI.DTO.Users;

public class UserLoginEvent : INotification
{
    public Guid UserId { get; set; }

    public UserLoginEvent(Guid userId)
    {
        UserId = userId;
    }
}