using Tradibit.Shared.DTO;
using Tradibit.SharedUI.DTO;

namespace Tradibit.Shared.Entities;

public class UserFund : BaseTrackableId 
{
    public Guid UserId { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Value { get; set; }
}