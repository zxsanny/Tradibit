using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Users;

namespace Tradibit.Shared.Entities;

public class User : BaseTrackableId
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string ImageUrl { get; set; }
    
    public string BinanceKeyHash { get; set; }
    public string BinanceSecretHash { get; set; }

    public List<UserPermission> Permissions { get; set; }
    
    public string BinanceKey => EncryptionService.Decrypt(BinanceKeyHash);
    public string BinanceSecret => EncryptionService.Decrypt(BinanceSecretHash);

    public UserSettings UserSettings { get; set; }
    
    public UserState UserState { get; set; }
    
    public ICollection<UserState> HistoryUserState { get; set; }
}
