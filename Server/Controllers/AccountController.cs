using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tradibit.Common.Entities;
using Tradibit.Common.Events;

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
        var user = await _mediator.Send(new GetUserQuery(emailClaim.Value));
        if (user == null)
        {
            await HttpContext.SignOutAsync();
            return Redirect($"/?error=No user in db!");
        }

        await _mediator.Publish(new UserLoginEvent(user.Id));
        await HttpContext.SignInAsync(GoogleDefaults.AuthenticationScheme, authRes.Principal);
        return Redirect(returnUrl ?? "/");
    }
}

public class GetUserQuery : IRequest<User>
{
    public string Email { get; set; }

    public GetUserQuery(string email)
    {
        Email = email;
    }
}

public interface IAccountApi
{
}

public class AuthConfig
{
    public GoogleAuth GoogleAuth { get; set; }
    public string Secret { get; set; }
    public int TokenExpInSeconds { get; set; }
}
    
public class GoogleAuth
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}