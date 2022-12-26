using Tradibit.SharedUI.DTO;

namespace Tradibit.Shared.Entities;

public class Scenario : BaseTrackableId
{
    public Guid UserId { get; set; }
    public Guid StrategyId { get; set; }
    
    public User User { get; set; }
    public Strategy Strategy { get; set; }
    
    public bool IsActive { get; set; }

    public ICollection<ScenarioState> ScenarioStates { get; set; }

    private Guid? _initialStep;
    public Guid InitialStep => _initialStep ??= Strategy.Steps.SingleOrDefault(x => x.IsInitial)!.Id;
    
    private Dictionary<Guid, Step> _stepsDict;
    public Dictionary<Guid, Step> StepsDict => _stepsDict ??= Strategy.Steps.ToDictionary(x => x.Id);
}