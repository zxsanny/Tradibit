using MediatR;
using Tradibit.Shared.Entities;

namespace Tradibit.Shared;

public class GetUserByIdRequest : IRequest<User?>
{
    public Guid UserId { get; set; }

    public GetUserByIdRequest(Guid userId)
    {
        UserId = userId;
    }
}

public class GetUserByEmailRequest : IRequest<User>
{
    public string Email { get; set; }

    public GetUserByEmailRequest(string email)
    {
        Email = email;
    }
}