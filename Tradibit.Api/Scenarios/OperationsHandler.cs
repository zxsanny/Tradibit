using MediatR;
using Tradibit.DataAccess;
using Tradibit.Shared.DTO.Primitives;
using Tradibit.Shared.DTO.UserBroker;
using Tradibit.Shared.Entities;

namespace Tradibit.Api.Scenarios;

public class OperationsHandler :
    IRequestHandler<OrderBaseOperation>,
    IRequestHandler<SetOperandBaseOperation>
{
    private readonly IMediator _mediator;
    private readonly TradibitDb _db;

    public OperationsHandler(IMediator mediator, TradibitDb db)
    {
        _mediator = mediator;
        _db = db;
    }
    
    public async Task<Unit> Handle(OrderBaseOperation request, CancellationToken cancellationToken)
    {
        var user = request.Scenario.User;
        
        var pairInterval = request.KlineUpdateEvent.PairInterval;
        if (request.OrderSide == OrderSide.BUY)
        {
            var maxTrades = user.UserSettings.MaxActiveTrades;
            var activeTrades = user.UserState.ActivePairs.Count;
            if (maxTrades == activeTrades)
                return Unit.Value;
            
            var amount = user.UserState.CurrentDeposit / ( maxTrades - activeTrades);
            var boughtAmount = await _mediator.Send(new BuyEvent(user.Id, pairInterval.Pair, amount), cancellationToken);
            user.UserState.ActivePairs.Add(new ActivePair(pairInterval, boughtAmount));
            user.UserState.CurrentDeposit -= amount;
            await _db.Save(user.UserState, cancellationToken);
        }
        else
        {
            var activePair = user.UserState.ActivePairs.FirstOrDefault(x => x.PairInterval == pairInterval);
            if (activePair == null)
                return Unit.Value;

            var soldAmount = await _mediator.Send(new SellEvent(user.Id, pairInterval.Pair, activePair.Amount), cancellationToken);
            user.UserState.ActivePairs.Remove(activePair);
            user.UserState.CurrentDeposit += soldAmount;
            await _db.Save(user.UserState, cancellationToken);
        }
        return Unit.Value;
    }

    public async Task<Unit> Handle(SetOperandBaseOperation request, CancellationToken cancellationToken)
    {
        var val = request.OperandSource.GetValue(request.Scenario, request.KlineUpdateEvent.QuoteIndicator);
        request.OperandTo.SetValue(request.Scenario, val);
        return await Task.FromResult(Unit.Value);
    }
}