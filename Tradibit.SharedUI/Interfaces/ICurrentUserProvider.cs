using Tradibit.SharedUI.DTO.Users;

namespace Tradibit.SharedUI.Interfaces;

public interface ICurrentUserProvider
{
    UserDto CurrentUser { get; }
}