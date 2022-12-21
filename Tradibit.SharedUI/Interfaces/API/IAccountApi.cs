using Refit;
using Tradibit.SharedUI.DTO;

namespace Tradibit.SharedUI.Interfaces.API;

public interface IAccountApi
{
    /// <summary> Gets current logged in user token  </summary>
    [Get("/account/getCurrentUserToken")]
    Task<Response<string>> GetCurrentUserToken();
}