namespace Tradibit.Common.Entities;

public class Scenario : BaseTrackableId
{
    public Guid UserId { get; set; }
    public Guid StrategyId { get; set; }
    
    public User User { get; set; }
    public Strategy Strategy { get; set; }
    public ScenarioState State { get; set; }
    
    public bool IsActive { get; set; }
    public decimal DepositPercent { get; set; }

    
    private Guid? _initialStep;
    public Guid InitialStep => _initialStep ??= Strategy.Steps.SingleOrDefault(x => x.IsInitial)!.Id;
    
    private Dictionary<Guid, Step> _stepsDict;
    public Dictionary<Guid, Step> StepsDict => _stepsDict ??= Strategy.Steps.ToDictionary(x => x.Id);
    
    public Step CurrentStep => _stepsDict[State.CurrentStepId];
}

public class ScenarioState
{
    public decimal DepositMoney { get; set; }
    public decimal PositionMoney { get; set; }
    public Dictionary<string, decimal?> UserVars { get; set; }
    
    public Guid CurrentStepId { get; set; }
        
    public Pair ActivePair { get; set; }
}