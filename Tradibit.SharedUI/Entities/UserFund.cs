using Tradibit.Common.DTO;
using Tradibit.Common.Entities;

namespace Tradibit.SharedUI.Entities;

public class UserFund : BaseTrackableId 
{
    public Guid UserId { get; set; }
    public TimeValue TimeValue { get; set; }
}