namespace Tradibit.Common.Entities;

public class StrategyUserPermissions : BaseTrackableId
{
    public Guid StrategyId { get; set; }
    public Guid UserId { get; set; }

    public Strategy Strategy { get; set; }
    public User User { get; set; }
}