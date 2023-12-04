using System.Collections.Concurrent;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.DataAccess;
using Tradibit.Shared;
using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Coins;
using Tradibit.Shared.DTO.Primitives;
using Tradibit.Shared.DTO.Scenarios;
using Tradibit.Shared.Entities;
using Z.EntityFramework.Plus;

namespace Tradibit.Api.Scenarios;

public class ScenarioWorker :
    INotificationHandler<AppInitEvent>,
    IRequestHandler<AddScenariosEvent>,
    IRequestHandler<RunBackTestEvent>,
    IRequestHandler<RemoveScenarioEvent>,
    IRequestHandler<KlineUpdateEvent>
{
    private readonly IMediator _mediator;
    private readonly TradibitDb _db;

    private static readonly ConcurrentDictionary<PairInterval, List<Scenario>> ActiveScenarios = new();
    private static readonly ConcurrentDictionary<Guid, List<Scenario>> ReplyHistoryScenarios = new();
    
    public ScenarioWorker(IMediator mediator, TradibitDb db)
    {
        _mediator = mediator;
        _db = db;
    }
    
    public async Task<Unit> Handle(AddScenariosEvent ev, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FindAsync(new object[] { ev.UserId }, cancellationToken: cancellationToken);
        if (user == null)
            throw new Exception($"There is no user in db by id {ev.UserId}");

        var scenarios = await GetScenarios(ev.StrategyId, user, ev.Pairs, ev.Intervals, cancellationToken);
        await _db.BulkInsertAsync(scenarios, cancellationToken);
        foreach (var scenario in scenarios)
        {
            if (ActiveScenarios.TryGetValue(scenario.PairInterval, out var activeScenarios))
                activeScenarios.Add(scenario);
            else
                ActiveScenarios[scenario.PairInterval] = new List<Scenario> { scenario };
        }
        return Unit.Value;
    }
    
    public async Task<Unit> Handle(RunBackTestEvent ev, CancellationToken cancellationToken)
    {
        var scenarios = await GetScenarios(ev.StrategyId, new User
        {
            UserState = new UserState{CurrentDeposit = ev.Deposit},
            UserSettings = new UserSettings{MaxActiveTrades = ev.MaxActiveTrades}
        }, ev.Pairs, ev.Intervals, cancellationToken);
        var pairIntervals = scenarios.Select(x => x.PairInterval).ToList();
        
        ReplyHistoryScenarios.TryAdd(ev.BackTestRunId, scenarios);
        await _mediator.Send(new ReplyHistoryEvent(ev.BackTestRunId, ev.HistorySpan, pairIntervals), cancellationToken);
        ReplyHistoryScenarios.TryRemove(ev.BackTestRunId, out _);
        
        return Unit.Value;
    }

    private async Task<List<Scenario>> GetScenarios(Guid strategyId, User user, 
        List<Pair>? pairs = null, 
        List<Interval>? intervals = null,
        CancellationToken cancellationToken = default)
    {
        var strategy = await _db.Strategies.FindAsync(new object[] { strategyId }, cancellationToken: cancellationToken);
        if (strategy == null)
            throw new Exception($"There is no strategy in db by id {strategyId}");
        
        pairs ??= await _mediator.Send(new GetMostCapCoinsRequest(), cancellationToken);
        var pairIntervals = pairs.ToPainIntervals(intervals ?? Constants.DefaultIntervals).ToList();

        return pairIntervals.Select(pairInterval => new Scenario(strategy, pairInterval, user)).ToList();
    }
    
    public async Task<Unit> Handle(RemoveScenarioEvent ev, CancellationToken cancellationToken) => 
        await RemoveScenarios(ev.StrategyId, ev.ScenarioId, cancellationToken);

    private async Task<Unit> RemoveScenarios(Guid? strategyId, Guid? scenarioId, CancellationToken cancellationToken = default)
    {
        foreach (var kv in ActiveScenarios) 
            kv.Value.RemoveAll(x => x.StrategyId == strategyId || x.Id == scenarioId);

        await _db.Scenarios
            .Where(x => x.StrategyId == strategyId || x.Id == scenarioId)
            .UpdateAsync(x => new Scenario
            {
                Status = ScenarioStatus.Stop
            }, cancellationToken);
        
        return Unit.Value;
    }

    public async Task Handle(AppInitEvent notification, CancellationToken cancellationToken)
    {
        var scenarios = await _db.Scenarios
            .Include(x => x.Strategy.Steps)
                .ThenInclude(x => x.Transitions)
                .ThenInclude(x => x.Conditions)
            .Include(x => x.Strategy.Steps)
                .ThenInclude(x => x.Transitions)
                .ThenInclude(x => x.SuccessOperations)
            .Include(scenario => scenario.PairInterval)
                .ThenInclude(x => x.Pair)
            .Include(scenario => scenario.PairInterval)
                .ThenInclude(x => x.Interval)
            .GroupBy(x => x.PairInterval)
            .ToDictionaryAsync(x => x.Key, v => v.ToList(), cancellationToken);
        
        foreach (var scenarioList in scenarios)
            ActiveScenarios.TryAdd(scenarioList.Key, scenarioList.Value);
    }

    public async Task<Unit> Handle(KlineUpdateEvent e, CancellationToken cancellationToken)
    {
        foreach (var scenario in ActiveScenarios.GetValueOrDefault(e.PairInterval) ?? new List<Scenario>()) 
            await ApplyKlineToScenario(scenario, e, cancellationToken);
        
        return Unit.Value;
    }

    private async Task ApplyKlineToScenario(Scenario scenario, KlineUpdateEvent e, CancellationToken cancellationToken)
    {
        bool transited;
        var currentStep = scenario.CurrentStep;

        do //could be multiple transition on 1 Kline update
        {
            transited = false;
            foreach (var transition in currentStep.Transitions)
            {
                if (!transition.MeetConditions(scenario, e.QuoteIndicator))
                    continue;

                await Task.WhenAll(transition.SuccessOperations
                    .OrderBy(x => x.OrderNo)
                    .Select(op =>
                    {
                        op.Scenario = scenario;
                        op.KlineUpdateEvent = e;
                        return _mediator.Send(op, cancellationToken);
                    }));

                scenario.CurrentStepId = transition.DestinationStepId;
                currentStep = await _db.Steps
                    .Include(x => x.Transitions)
                    .ThenInclude(x => x.Conditions)
                    .Include(x => x.Transitions)
                    .ThenInclude(x => x.SuccessOperations)
                    .SingleAsync(x => x.Id == transition.DestinationStepId, cancellationToken);
                await _db.Save(scenario, cancellationToken);
                transited = true;
                break;
            }
        } while (transited);
    }
}
