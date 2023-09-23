namespace Tradibit.SharedUI.DTO.Primitives;

public class IdName
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public IdName(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}