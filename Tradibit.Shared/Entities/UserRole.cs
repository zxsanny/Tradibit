using Tradibit.Shared.DTO.Users;

namespace Tradibit.Shared.Entities;

public class UserRole
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<UserPermission> Permissions { get; set; }
}