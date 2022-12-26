using Tradibit.SharedUI.DTO;

namespace Tradibit.Shared.Entities;

//public - open to use for everyone 
//private - record in this table with CanEdit true
//shared by link - on link open create a link in this table with CanEdit depending on sharing status

public class StrategyUserVisibility : BaseTrackableId
{
    public Guid StrategyId { get; set; }
    public Guid UserId { get; set; }
    public bool CanEdit { get; set; }

    public Strategy Strategy { get; set; }
    public User User { get; set; }
}