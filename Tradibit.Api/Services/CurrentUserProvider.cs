using System.Security.Authentication;
using Duende.IdentityServer.Models;
using Newtonsoft.Json;
using Tradibit.Shared.DTO.Users;
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

    public Guid CurrentUserId
    {
        get
        {
            if (!Guid.TryParse(_httpAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == nameof(UserDto.Id))?.Value, out var userId))
                userId = Guid.Empty;
            return userId;
        }
    }
    
    public UserDto CurrentUser
    {
        get
        {
            var principal = _httpAccessor.HttpContext?.User; 
            if (principal == null)
                throw new AuthenticationException("Auth Claims are empty!");
        
            var claimsDict = principal.Claims.ToDictionary(x => x.Type, x => x.Value);

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