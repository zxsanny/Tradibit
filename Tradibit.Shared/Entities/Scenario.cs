using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.Shared.Entities;

public class Scenario : BaseTrackableId
{
    public Guid StrategyId { get; set; }
    public PairIntervalKey PairIntervalKey { get; set; }
    public Dictionary<string, decimal?> UserVars { get; set; }
    public Guid CurrentStepId { get; set; }

    public Strategy Strategy { get; set; }
    
    
    private Guid? _initialStepId;
    public Guid InitialStep => _initialStepId ??= Strategy.Steps.SingleOrDefault(x => x.IsInitial)!.Id;
    
    public Step CurrentStep => StepsDict[CurrentStepId];
    
    private Dictionary<Guid, Step> _stepsDict;
    public Dictionary<Guid, Step> StepsDict => _stepsDict ??= Strategy.Steps.ToDictionary(x => x.Id);
}