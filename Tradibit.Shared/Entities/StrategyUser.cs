using Tradibit.SharedUI.DTO;

namespace Tradibit.Shared.Entities;

public class StrategyUser : BaseTrackableId
{
    public Guid StrategyId { get; set; }
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    
    public Strategy Strategy { get; set; }
    public User User { get; set; }
}