using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.Shared.Entities;

public class Scenario : BaseTrackableId
{
    public Guid StrategyId { get; set; }
    public Pair Pair { get; set; }
    public Dictionary<string, decimal?> UserVars { get; set; }
    public Guid CurrentStepId { get; set; }

    public Strategy Strategy { get; set; }
    
    
    private Dictionary<Guid, Step> _stepsDict;
    public Dictionary<Guid, Step> StepsDict => _stepsDict ??= Strategy.Steps.ToDictionary(x => x.Id);
    public Step CurrentStep => StepsDict[CurrentStepId];
}