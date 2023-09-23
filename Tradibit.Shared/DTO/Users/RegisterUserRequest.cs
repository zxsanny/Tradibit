using MediatR;

namespace Tradibit.SharedUI.DTO.Users;

public class RegisterUserRequest : IRequest<string>
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? ImageUrl { get; set; }
}