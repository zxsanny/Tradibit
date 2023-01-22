using MediatR;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.Shared.Entities;

public abstract class OperationBase : BaseTrackableId, IRequest
{
    public int OrderNo { get; set; }

    public Guid TransitionId { get; set; }
    public Transition Transition { get; set; }
    
    // non trackable
    public Scenario Scenario { get; set; }
    public KlineUpdateEvent KlineUpdateEvent { get; set; }
}

public class OrderOperation : OperationBase
{
    public OrderSide OrderSide { get; set; }
}

public class SetOperandOperation : OperationBase
{
    public Guid OperandSourceId { get; set; }
    public Operand OperandSource { get; set; }

    public Guid OperandToId { get; set; }
    public Operand OperandTo { get; set; }
}