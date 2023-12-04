using System.Reflection;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tradibit.Shared.DTO.Auth;
using Tradibit.Shared.DTO.Users;
using Tradibit.Shared.Entities;
using Tradibit.SharedUI.DTO.Users;

namespace Tradibit.Api.Controllers;

[ApiExplorerSettings(IgnoreApi=true)]
public class AccountController : TradibitBaseController
{
    private readonly IMediator _mediator;
    private readonly AuthConfig _authConfig;
    
    public AccountController(IMediator mediator, ILogger<AccountController> logger, IOptions<AuthConfig> authConfig) : base(mediator, logger)
    {
        _mediator = mediator;
        _authConfig = authConfig.Value;
    }

    [HttpPost]
    [Route("register-google-user")]
    public async Task<string> RegisterGoogleUser(RegisterUserRequest userRequest) => 
        await Send(userRequest);
    
    private IEnumerable<Claim> GetClaims(User user)
    {
        var claims = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.CanWrite)
            .Select(x =>
            {
                var valStr = x.GetValue(this) switch
                {
                    List<UserPermission> permissions => JsonConvert.SerializeObject(permissions),
                    Guid guid => guid.ToString(),
                    string s => s,
                    _ => ""
                };
                return new Claim(x.Name, valStr);
            })
            .ToList();
        claims.Add(new Claim(ClaimTypes.Name, user.Name!));
        return claims;
    }
}/**/