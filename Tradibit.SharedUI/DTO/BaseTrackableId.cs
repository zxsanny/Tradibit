namespace Tradibit.SharedUI.DTO;

public abstract class BaseTrackableId
{
    public Guid Id { get; set; }
    
    public DateTime CreatedDateTime { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedDateTime { get; set; }
    public Guid? ModifiedBy { get; set; }
}