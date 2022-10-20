using MediatR;

namespace Tradibit.Common.Events;

public class UserLoginEvent : INotification
{
    public Guid UserId { get; set; }

    public UserLoginEvent(Guid userId)
    {
        UserId = userId;
    }
}