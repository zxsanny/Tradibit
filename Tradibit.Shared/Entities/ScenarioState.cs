using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.Shared.Entities;

public class ScenarioState
{
    public Guid ScenarioId { get; set; }
    public Pair Pair { get; set; }
    public Guid CurrentStepId { get; set; }
    
    public Dictionary<string, decimal?> UserVars { get; set; }

    public Scenario Scenario { get; set; }
}