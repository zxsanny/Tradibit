using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO.Dashboard;

public class Operation
{
    public TimeValue? TimeValue { get; set; }
    public OrderSide? OrderSide { get; set; }
    public string? Name { get; set; }
}