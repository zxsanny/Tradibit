namespace Tradibit.SharedUI.DTO.Dashboard;

public class UserDashboard
{
    public List<TimeValue>? UserFunds { get; set; }
    public ProfitLossStat? UserStat { get; set; }
    public List<ScenarioDto>? Scenarios { get; set; }
}