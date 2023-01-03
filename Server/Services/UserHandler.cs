using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.DataAccess;
using Tradibit.Shared.Entities;
using Tradibit.Shared.Events;

namespace Tradibit.Api.Services;

public class UserHandler : IRequestHandler<GetUserByIdRequest, User>
{
    private readonly IMediator _mediator;
    private readonly TradibitDb _db;

    public UserHandler(IMediator mediator, TradibitDb db)
    {
        _mediator = mediator;
        _db = db;
    }
    
    public async Task<User> Handle(GetUserByIdRequest request, CancellationToken cancellationToken) =>
        await _db.Users
            .Include(x => x.UserState)
            .Include(x => x.UserSettings)
            .Include(x => x.HistoryUserState)
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
}