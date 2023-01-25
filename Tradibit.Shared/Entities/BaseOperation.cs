using MediatR;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.Shared.Entities;

public abstract class BaseOperation : BaseTrackableId, IRequest
{
    public int OrderNo { get; set; }

    public Guid TransitionId { get; set; }
    public Transition Transition { get; set; }
    
    // non trackable
    public Scenario Scenario { get; set; }
    public KlineUpdateEvent KlineUpdateEvent { get; set; }
}

public class OrderBaseOperation : BaseOperation
{
    public OrderSide OrderSide { get; set; }
}

public class SetOperandBaseOperation : BaseOperation
{
    public Operand OperandSource { get; set; }
    public Operand OperandTo { get; set; }
}