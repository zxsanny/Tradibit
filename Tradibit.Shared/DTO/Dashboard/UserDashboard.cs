using Binance.Net.Enums;

namespace Tradibit.Common.DTO.Dashboard;

public class UserDashboard
{
    public List<TimeValue> UserFunds { get; set; }
    public ProfitLossStat UserStat { get; set; }
    public List<ScenarioDto> Scenarios { get; set; }
}

public class ScenarioDto
{
    public Guid ScenarioId { get; set; }
    public string Name { get; set; }
    public ProfitLossStat Stat { get; set; }
    
    public List<Operation> OngoingOperation { get; set; }
    public List<List<Operation>> History { get; set; }
}

public class Operation
{
    public TimeValue TimeValue { get; set; }
    public OrderSide? OrderSide { get; set; }
    public string Name { get; set; }
}