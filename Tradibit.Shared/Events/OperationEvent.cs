using MediatR;
using Tradibit.Shared.Entities;

namespace Tradibit.Shared.Events;

public class OperationEvent : IRequest
{
    public OperationBase OperationBase { get; set; }

    public OperationEvent(OperationBase operationBase)
    {
        OperationBase = operationBase;
    }
}