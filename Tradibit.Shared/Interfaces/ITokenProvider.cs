using System.Security.Claims;

namespace Tradibit.SharedUI.Interfaces;

public interface ITokenProvider
{
    Task<string?> GetToken(CancellationToken cancellationToken = default);
    Task<IEnumerable<Claim>> GetClaims(CancellationToken cancellationToken = default);
    Task<bool> IsSuperAdmin(CancellationToken cancellationToken = default);
    Task<Guid> GetCurrentUserId(CancellationToken cancellationToken = default);
}
