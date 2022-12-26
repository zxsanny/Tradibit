using System.Security.Claims;

namespace Tradibit.SharedUI.Interfaces;

public interface ITokenProvider
{
    Task<string?> GetToken();
    Task<IEnumerable<Claim>> GetClaims();
    Task<bool> IsSuperAdmin();
}
