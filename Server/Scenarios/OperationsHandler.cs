using MediatR;
using Tradibit.Shared.Events;

namespace Tradibit.Api.Scenarios;

public class OperationsHandler : IRequestHandler<OperationEvent>
{
    public Task<Unit> Handle(OperationEvent request, CancellationToken cancellationToken)
    {
        request.OperationBase
    }
}