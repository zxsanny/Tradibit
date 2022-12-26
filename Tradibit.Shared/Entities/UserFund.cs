using Tradibit.SharedUI.DTO;

namespace Tradibit.Shared.Entities;

public class UserFund : BaseTrackableId 
{
    public Guid UserId { get; set; }
    public TimeValue TimeValue { get; set; }
}