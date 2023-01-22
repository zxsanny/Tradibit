using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.DataAccess;
using Tradibit.Shared.Entities;
using Tradibit.SharedUI.DTO.Primitives;
using Tradibit.SharedUI.DTO.UserBroker;

namespace Tradibit.Api.Scenarios;

public class OperationsHandler :
    IRequestHandler<OrderOperation>,
    IRequestHandler<SetOperandOperation>
{
    private readonly IMediator _mediator;
    private readonly TradibitDb _db;

    public OperationsHandler(IMediator mediator, TradibitDb db)
    {
        _mediator = mediator;
        _db = db;
    }
    
    public async Task<Unit> Handle(OrderOperation request, CancellationToken cancellationToken)
    {
        var users = await _db.StrategyUsers
            .Where(x => x.StrategyId == request.Scenario.StrategyId)
            .Select(x => x.User)
            .Include(x => x.UserState)
            .Include(x => x.UserSettings)
            .ToListAsync(cancellationToken);
        
        foreach (var user in users)
        {
            var pair = request.KlineUpdateEvent.PairIntervalKey.Pair;
            if (request.OrderSide == OrderSide.BUY)
            {
                var maxTrades = user.UserSettings.MaxActiveTradings;
                var activeTrades = user.UserState.ActivePairs.Count;
                if (maxTrades == activeTrades)
                    continue;
                
                var amount = user.UserState.CurrentDeposit / ( maxTrades - activeTrades);
                var boughtAmount = await _mediator.Send(new BuyEvent(user.Id, request.KlineUpdateEvent.PairIntervalKey.Pair, amount), cancellationToken);
                user.UserState.ActivePairs.Add(new ActivePair(pair, boughtAmount));
                user.UserState.CurrentDeposit -= amount;
                await _db.Save(user.UserState, cancellationToken);
            }
            else
            {
                var activePair = user.UserState.ActivePairs.FirstOrDefault(x => x.Pair == pair);
                if (activePair == null)
                    continue;

                var soldAmount = await _mediator.Send(new SellEvent(user.Id, pair, activePair.Amount), cancellationToken);
                user.UserState.ActivePairs.Remove(activePair);
                user.UserState.CurrentDeposit += soldAmount;
                await _db.Save(user.UserState, cancellationToken);
            }
        }
        return Unit.Value;
    }

    public async Task<Unit> Handle(SetOperandOperation request, CancellationToken cancellationToken)
    {
        var val = request.OperandSource.GetValue(request.Scenario, request.KlineUpdateEvent.QuoteIndicator);
        request.OperandTo.SetValue(request.Scenario, val);
        return await Task.FromResult(Unit.Value);
    }
}