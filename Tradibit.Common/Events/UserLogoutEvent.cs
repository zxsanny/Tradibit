using MediatR;

namespace Tradibit.Common.Events;

public class UserLogoutEvent : INotification
{
    public Guid UserId { get; set; }
}