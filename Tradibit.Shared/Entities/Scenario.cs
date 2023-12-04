using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.Entities;

public class Scenario : BaseTrackableId
{
    public Guid  StrategyId { get; set; }
    public Guid? UserId { get; set; }
    public PairInterval PairInterval { get; set; }
    
    public Dictionary<string, decimal?>? UserVars { get; set; }
    
    public Strategy Strategy { get; set; }
    public User User { get; set; }
    
    public Guid CurrentStepId { get; set; }
    public Step CurrentStep => StepsDict[CurrentStepId];

    private Dictionary<Guid, Step>? _stepsDict;
    public Dictionary<Guid, Step> StepsDict => _stepsDict ??= Strategy.Steps.ToDictionary(x => x.Id);

    public ScenarioStatus Status;
    
    //for ef core
    public Scenario(){}
    
    public Scenario(Strategy strategy, PairInterval pairInterval, User user)
    {
        StrategyId = strategy.Id;
        Strategy = strategy;
        PairInterval = pairInterval;
        UserId = user.Id;
        User = user;
        UserVars = new Dictionary<string, decimal?>();
        CurrentStepId = strategy.InitialStepId;
    }
}