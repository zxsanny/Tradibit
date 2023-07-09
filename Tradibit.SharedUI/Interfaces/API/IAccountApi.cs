using Refit;
using Tradibit.SharedUI.DTO.Users;

namespace Tradibit.SharedUI.Interfaces.API;

public interface IAccountApi
{
    [Post("/api/account/register-google-user")]
    Task<string> RegisterGoogleUser(RegisterUserRequest userRequest);
}