using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Users;

namespace Tradibit.SharedUI.DTO.Users;

public class UserDto : BaseTrackableId
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? ImageUrl { get; set; }
    
    public string? BinanceKeyHash { get; set; }
    public string? BinanceSecretHash { get; set; }

    public List<UserPermission>? Permissions { get; set; }
}