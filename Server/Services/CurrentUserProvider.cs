using System.Security.Authentication;
using Duende.IdentityServer.Models;
using Newtonsoft.Json;
using Tradibit.SharedUI.DTO.Users;
using Tradibit.SharedUI.Interfaces;

namespace Tradibit.Api.Services;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpAccessor)
    {
        _httpAccessor = httpAccessor;
    }

    public UserDto CurrentUser
    {
        get
        {
            var principal = _httpAccessor.HttpContext?.User; 
            if (principal == null)
                throw new AuthenticationException("Auth Claims are empty!");
        
            var claims = principal.Identities.SelectMany(x => x.Claims).ToList();
            if (!claims.Any())
                return null;

            var claimsDict = claims.ToDictionary(x => x.Type, x => x.Value);

            return new UserDto
            {
                Id = Guid.Parse(claimsDict[nameof(UserDto.Id)]),
                Name = claimsDict[nameof(UserDto.Name)],
                Email = claimsDict[nameof(IdentityResources.Email)],
                BinanceKeyHash = claimsDict[nameof(UserDto.BinanceKeyHash)],
                BinanceSecretHash = claimsDict[nameof(UserDto.BinanceSecretHash)],
                Permissions = JsonConvert.DeserializeObject<List<UserPermission>>(claimsDict[nameof(UserDto.Permissions)]) ?? new List<UserPermission>()
            };
        }
    }
}