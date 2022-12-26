namespace Tradibit.SharedUI.DTO.Dashboard;

public class ScenarioDto
{
    public Guid ScenarioId { get; set; }
    public string? Name { get; set; }
    public ProfitLossStat? Stat { get; set; }
    
    public List<Operation>? OngoingOperation { get; set; }
    public List<List<Operation>>? History { get; set; }
}