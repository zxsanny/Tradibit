using Tradibit.SharedUI.DTO;

namespace Tradibit.Shared.Entities;

public class UserSettings : BaseTrackableId
{
    public Guid UserId { get; set; }

    public int MaxActiveTradings { get; set; }
}