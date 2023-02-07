using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.Extensions;

namespace Tradibit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public abstract class TradibitBaseController : ControllerBase
{
    protected readonly ILogger Logger;
    protected readonly IMediator Mediator;
    
    public TradibitBaseController(IMediator mediator, ILogger logger)
    {
        Mediator = mediator;
        Logger = logger;
    }
    
    protected async Task<Response<T>> Send<T>(IRequest<T> request)
    {
        try
        {
            var res = await Mediator.Send(request);
            if (res == null)
                throw new Exception($"Request {request.GetType().Name}: {JsonConvert.SerializeObject(request)} returns NULL!");
            return new Response<T>(res);
        }
        catch (Exception e)
        {
            Logger.LogError(e, e.GetAllMessages());
            throw;
        }
    }
}