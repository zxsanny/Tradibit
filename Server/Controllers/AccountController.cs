using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Tradibit.Shared.Entities;
using Tradibit.Shared.Events;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.SettingsDTO;
using Tradibit.SharedUI.DTO.Users;
using Tradibit.SharedUI.Interfaces.API;

namespace Tradibit.Api.Controllers;

[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi=true)]
public class AccountController : Controller, IAccountApi
{
    private readonly IMediator _mediator;
    private readonly AuthConfig _authConfig;
    
    public AccountController(IMediator mediator, IOptions<AuthConfig> authConfig)
    {
        _mediator = mediator;
        _authConfig = authConfig.Value;
    }

    [HttpGet("login")]
    [AllowAnonymous]
    public async Task Login(string returnUrl) =>
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(LoginCallback), new {returnUrl})
            });

    [HttpGet("Callback")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginCallback(string returnUrl)
    {
        var authRes = await HttpContext.AuthenticateAsync();
        if (!authRes.Succeeded || authRes.Principal == null)
            throw new Exception("Google Auth is failed!");

        var emailClaim = authRes.Principal.FindFirst(ClaimTypes.Email);
        if (emailClaim == null) return BadRequest();
        var user = await _mediator.Send(new GetUserRequest(emailClaim.Value));
        if (user == null)
        {
            await HttpContext.SignOutAsync();
            return Redirect($"/?error=No user in db!");
        }

        await _mediator.Publish(new UserLoginEvent(user.Id));
        
        //Claims can't be added to existing authResult.Principal, should be cloned first, specific of .net core
        var principal = authRes.Principal.Clone();
        var identity = (ClaimsIdentity)principal.Identity;
        identity?.TryRemoveClaim(identity.FindFirst(x => x.Type == ClaimTypes.Name)); //remove Name claim, need to set another name from loyalty db
        identity?.TryRemoveClaim(identity.FindFirst(x => x.Type == ClaimTypes.Role)); //remove Role claim, need to set another role from loyalty db
        identity?.AddClaims(GetClaims(user));
        
        await HttpContext.SignInAsync(GoogleDefaults.AuthenticationScheme, authRes.Principal);
        return Redirect(returnUrl ?? "/");
    }

    public async Task<Response<string>> GetCurrentUserToken()
    {
        if (User.Identity is not {IsAuthenticated: true}) 
            return new Response<string>("");
            
        var token = new JwtSecurityTokenHandler
        {
            //Crucial in order to not change claim types during token creation
            OutboundClaimTypeMap = new Dictionary<string, string>() 
        }.CreateJwtSecurityToken(
            new SecurityTokenDescriptor
            {
                Subject = (ClaimsIdentity)User.Identity,
                Expires = DateTime.UtcNow.Add(TimeSpan.FromSeconds(_authConfig.TokenExpInSeconds)),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authConfig.Secret)),
                    SecurityAlgorithms.HmacSha256Signature)
            });
            
        return await Task.FromResult(new Response<string>(token.RawData));
    }

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
}