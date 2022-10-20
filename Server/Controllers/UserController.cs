using MediatR;

namespace Tradibit.Api.Controllers;

public class UserController : TradibitBaseController
{
    public UserController(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }
}