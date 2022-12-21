using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.DTO.Queries;
using Tradibit.Common.Interfaces.API;
using Tradibit.Common.SettingsDTO;
using Tradibit.SharedUI.Interfaces.API;

namespace Tradibit.Api.Controllers;

[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi=true)]
public class AccountController : Controller, IAccountApi
{
    private readonly IMediator _mediator;
    
    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
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