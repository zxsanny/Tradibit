using MediatR;

namespace Tradibit.Common.DTO.Events;

public class UserLogoutEvent : INotification
{
    public Guid UserId { get; set; }
}