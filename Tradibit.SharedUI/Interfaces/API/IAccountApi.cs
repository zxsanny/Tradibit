using Refit;
using Tradibit.SharedUI.DTO;

namespace Tradibit.SharedUI.Interfaces.API;

public interface IAccountApi
{
    [Get("/account/getCurrentUserToken")]
    Task<Response<string>> GetCurrentUserToken();
}