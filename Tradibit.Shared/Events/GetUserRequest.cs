using MediatR;
using Tradibit.Shared.Entities;

namespace Tradibit.Shared.Events;

public class GetUserRequest : IRequest<User?>
{
    public string Email { get; set; }

    public GetUserRequest(string email)
    {
        Email = email;
    }
}