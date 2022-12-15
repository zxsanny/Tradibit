using System.Collections.Concurrent;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.Entities;
using Tradibit.Common.Interfaces;
using Tradibit.DataAccess;

namespace Tradibit.Api.Scenarios;

public class ScenarioWorker : 
    INotificationHandler<UserLoginEvent>,
    IRequestHandler<StartScenarioEvent>,
    IRequestHandler<StartHistoryTestScenarioEvent>,
    IRequestHandler<StopScenarioEvent>,
    IRequestHandler<KlineUpdateEvent>
{
    private readonly IUserBrokerService _userBrokerService;
    private readonly IMediator _mediator;
    private readonly TradibitDb _db;

    private static readonly ConcurrentDictionary<Guid, Scenario> ActiveScenariosDict = new();
    private static readonly ConcurrentDictionary<Guid, Scenario> ReplyHistoryScenariosDict = new();
    private static bool _init = false;

    private static ConcurrentDictionary<Guid, Scenario> GetScenarios(bool isHistory) =>
        isHistory ? ReplyHistoryScenariosDict : ActiveScenariosDict;

    public ScenarioWorker(IUserBrokerService userBrokerService, IMediator mediator, TradibitDb db)
    {
        _userBrokerService = userBrokerService;
        _mediator = mediator;
        _db = db;
    }
    
    public async Task<Unit> Handle(StartScenarioEvent request, CancellationToken cancellationToken)
    {
        var scenario = await _db.Scenarios.FindAsync(request.ScenarioId, cancellationToken);
        var deposit = await _userBrokerService.GetUsdtBalance(cancellationToken) * scenario.DepositPercent;
        StartScenario(false, scenario, deposit, cancellationToken);
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
        scenario.State = new ScenarioState 
        {
            DepositMoney = deposit,
            CurrentStepId = scenario.InitialStep
        };
        GetScenarios(isHistory).TryAdd(scenario.Id, scenario);
    }

    public async Task Handle(UserLoginEvent notification, CancellationToken cancellationToken)
    {
        if (_init)
            return;
        var scenarios = await _db.Scenarios.Where(s => s.IsActive).ToListAsync(cancellationToken);
        var scDict = GetScenarios(false);
        
        foreach (var scenario in scenarios) 
            scDict.TryAdd(scenario.Id, scenario);
        
        _init = true;
    }

    public async Task<Unit> Handle(KlineUpdateEvent e, CancellationToken cancellationToken)
    {
        var scenarios = GetScenarios(e.IsHistory);
        foreach (var scenario in scenarios)
        {
            bool transited;
            do //could be multiple transition on 1 Kline update
            {
                transited = false;
                foreach (var transition in scenario.Value.CurrentStep.Transitions)
                {
                    transited = await transition.TryTransit(scenario.Value);
                    if (transited)
                    {
                        await _db.Save(scenario.Value, cancellationToken);
                        break;
                    }
                }
            } while (transited);    
        }
        return Unit.Value;
    }

    public async Task<Unit> Handle(StopScenarioEvent request, CancellationToken cancellationToken)
    {
        GetScenarios(request.IsHistory).TryRemove(request.ScenarioId, out _);
        return Unit.Value;
    }
}
