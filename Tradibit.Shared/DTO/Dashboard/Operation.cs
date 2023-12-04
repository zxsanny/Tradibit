using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.DTO.Dashboard;

public class Operation
{
    public TimeValue? TimeValue { get; set; }
    public OrderSide? OrderSide { get; set; }
    public string? Name { get; set; }
}