using System.Collections.Concurrent;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.DataAccess;
using Tradibit.Shared.Entities;
using Tradibit.Shared.Events;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Scenarios;
using Tradibit.SharedUI.Interfaces;

namespace Tradibit.Api.Scenarios;

public class ScenarioWorker :
    INotificationHandler<AppInitEvent>,
    IRequestHandler<StartScenarioEvent>,
    IRequestHandler<StartHistoryTestScenarioEvent>,
    IRequestHandler<StopScenarioEvent>,
    IRequestHandler<KlineUpdateEvent>,
    IRequestHandler<KlineHistoryUpdateEvent>
{
    private readonly ICandlesProvider _candlesProvider;
    private readonly IMediator _mediator;
    private readonly TradibitDb _db;

    private static readonly ConcurrentDictionary<Guid, Scenario> ActiveScenariosDict = new();
    private static readonly ConcurrentDictionary<Guid, Scenario> ReplyHistoryScenariosDict = new();
    
    public ScenarioWorker(ICandlesProvider candlesProvider, IMediator mediator, TradibitDb db)
    {
        _candlesProvider = candlesProvider;
        _mediator = mediator;
        _db = db;
    }

    public async Task<Unit> Handle(StartScenarioEvent request, CancellationToken cancellationToken)
    {
        var scenario = await _db.Scenarios.FindAsync(request.ScenarioId, cancellationToken);
        
        // var balances = await _mediator.Send(new GetBalancesEvent(request.UserId, Currency.USDT), cancellationToken);
        //
        // var usdt = balances.FirstOrDefault(x => x.Key == Currency.USDT).Value;
        // if (usdt < 100)
        //     throw new Exception("No USDT funds found or USDT funds < $100");
        //
        // var totalFundsBtc = balances.Aggregate(0m, (s, balance) => s + balance.Value);
        // var totalUSDT = _candlesProvider.BtcValue * totalFundsBtc;
        // var requiredUSDT = totalUSDT * scenario.DepositPercent / 100;
        //
        // if (usdt < requiredUSDT)
        //     throw new Exception($"Available USDT funds is less then required USDT for the scenario! " +
        //                         $"Available USDT: {usdt}. Required for scenario: {requiredUSDT}, scenario percent: {scenario.DepositPercent}%");
        //     
        //StartScenario(false, scenario, requiredUSDT, cancellationToken);
        return Unit.Value;
    }

    public async Task<Unit> Handle(StartHistoryTestScenarioEvent request, CancellationToken cancellationToken)
    {
        var scenario = await _db.Scenarios.FindAsync(request.ScenarioId, cancellationToken);
        StartScenario(true, scenario, request.Deposit, cancellationToken);
        await _mediator.Send(new ReplyHistoryEvent(request.HistorySpan, request.Pairs, request.Intervals), cancellationToken);
        return Unit.Value;
    }

    private void StartScenario(bool isHistory, Scenario scenario, decimal deposit, CancellationToken cancellationToken = default)
    {
        // scenario.State = new ScenarioState
        // {
        //     DepositMoney = deposit,
        //     CurrentStepId = scenario.InitialStep
        // };
        // var scenarios = isHistory ? ReplyHistoryScenariosDict : ActiveScenariosDict;
        //scenarios.TryAdd(scenario.Id, scenario);
    }

    public async Task Handle(AppInitEvent notification, CancellationToken cancellationToken)
    {
        var scenarios = await _db.Scenarios.Where(s => s.Strategy.IsActive).ToListAsync(cancellationToken);
        foreach (var scenario in scenarios)
            ActiveScenariosDict.TryAdd(scenario.Id, scenario);
    }
    
    public async Task<Unit> Handle(KlineUpdateEvent e, CancellationToken cancellationToken)
    {
        foreach (var scenario in ActiveScenariosDict) 
            await MoveNext(scenario.Value, e.QuoteIndicator, cancellationToken);

        return Unit.Value;
    }

    private async Task MoveNext(Scenario scenario, QuoteIndicator quoteIndicator, CancellationToken cancellationToken)
    {
        bool transited;
        do //could be multiple transition on 1 Kline update
        {
            transited = false;
            foreach (var transition in scenario.CurrentStep.Transitions)
            {
                transited = await transition.TryTransit(scenario, quoteIndicator);
                if (transited)
                {
                    await _db.Save(scenario, cancellationToken);
                    break;
                }
            }
        } while (transited);
    }


    public async Task<Unit> Handle(KlineHistoryUpdateEvent e, CancellationToken cancellationToken)
    {
        var scenario = ReplyHistoryScenariosDict[e.ScenarioId];
        if (scenario == null)
            return Unit.Value;
        
        await MoveNext(scenario, e.QuoteIndicator, cancellationToken);
        return Unit.Value;
    }
    
    public async Task<Unit> Handle(StopScenarioEvent request, CancellationToken cancellationToken)
    {
        var scenarios = request.IsHistory ? ReplyHistoryScenariosDict : ActiveScenariosDict;
        scenarios.TryRemove(request.ScenarioId, out _);
        return await Task.FromResult(Unit.Value);
    }
}
