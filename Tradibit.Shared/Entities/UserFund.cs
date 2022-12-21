using Tradibit.Common.DTO;

namespace Tradibit.Common.Entities;

public class UserFund : BaseTrackableId 
{
    public Guid UserId { get; set; }
    public TimeValue TimeValue { get; set; }
}