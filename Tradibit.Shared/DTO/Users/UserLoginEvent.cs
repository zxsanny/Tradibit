using MediatR;

namespace Tradibit.Shared.DTO.Users;

public class UserLoginEvent : INotification
{
    public Guid UserId { get; set; }

    public UserLoginEvent(Guid userId)
    {
        UserId = userId;
    }
}