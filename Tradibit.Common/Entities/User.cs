using System.Reflection;
using System.Security.Authentication;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Tradibit.Common.Entities;

public class User : BaseTrackableId
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string BinanceKeyHash { get; set; }
    public string BinanceSecretHash { get; set; }

    public List<Permission> Permissions { get; set; }
    
    public string BinanceKey => EncryptionService.Decrypt(BinanceKeyHash);
    public string BinanceSecret => EncryptionService.Decrypt(BinanceSecretHash);
    
    public User(ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new AuthenticationException("Auth Claims are empty!");
        
        var claims = principal.Identities.SelectMany(x => x.Claims).ToList();
        if (!claims.Any())
            return;

        var claimsDict = claims.ToDictionary(x => x.Type, x => x.Value);
        Id = Guid.Parse(claimsDict[nameof(Id)]);
        Name = claimsDict[nameof(Name)];
        Email = claimsDict[nameof(Email)];
        BinanceKeyHash = claimsDict[nameof(BinanceKeyHash)];
        BinanceSecretHash = claimsDict[nameof(BinanceSecretHash)];
        Permissions = JsonConvert.DeserializeObject<List<Permission>>(claimsDict[nameof(Permissions)]);
    }
    
    public IEnumerable<Claim> ToClaims()
    {
        var claims = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.CanWrite)
            .Select(x =>
            {
                var valStr = x.GetValue(this) switch
                {
                    List<Permission> permissions => JsonConvert.SerializeObject(permissions),
                    Guid guid => guid.ToString(),
                    string s => s,
                    _ => ""
                };
                return new Claim(x.Name, valStr);
            })
            .ToList();
        claims.Add(new Claim(ClaimTypes.Name, Name));
        return claims;
    }
}

public enum Permission
{
    UsersAdministration = 10,
    
}