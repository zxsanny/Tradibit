using System.Reflection;
using System.Security.Claims;
using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Users;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Users;

namespace Tradibit.Shared.Entities;

public class User : BaseTrackableId
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string ImageUrl { get; set; }
    
    public string BinanceKeyHash { get; set; }
    public string BinanceSecretHash { get; set; }

    public List<UserRole> Roles { get; set; }
    public List<Strategy> UserStrategies { get; set; }
    
    public string BinanceKey => EncryptionService.Decrypt(BinanceKeyHash);
    public string BinanceSecret => EncryptionService.Decrypt(BinanceSecretHash);

    public UserSettings UserSettings { get; set; }
    public UserState UserState { get; set; }
    
    public IEnumerable<Claim> ToClaims()
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, Id.ToString()),
            new (ClaimTypes.Name, Name),
            new (ClaimTypes.Email, Email),
        };
        
        return claims;
    }
}
