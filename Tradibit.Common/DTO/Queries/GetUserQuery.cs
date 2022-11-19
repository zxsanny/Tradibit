using MediatR;
using Tradibit.Common.Entities;

namespace Tradibit.Common.DTO.Queries;

public class GetUserQuery : IRequest<User>
{
    public string Email { get; set; }

    public GetUserQuery(string email)
    {
        Email = email;
    }
}