using Tradibit.Shared.DTO;
using Tradibit.SharedUI.DTO;

namespace Tradibit.Shared.Entities;

public class UserSettings : BaseTrackableId
{
    public Guid UserId { get; set; }

    public int MaxActiveTrades { get; set; }
}