using Tradibit.SharedUI.DTO.Users;

namespace Tradibit.SharedUI.Interfaces;

public interface ICurrentUserProvider
{
    Guid CurrentUserId { get; }
    UserDto CurrentUser { get; }
}