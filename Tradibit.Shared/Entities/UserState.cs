using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.Shared.Entities;

public class UserState : BaseTrackableId
{
    public List<(Pair, decimal)> ActivePairs { get; set; }
    public decimal CurrentDeposit { get; set; }
    public Guid? StrategyId { get; set; }

    public UserState(decimal deposit, Guid? strategyId = null)
    {
        CurrentDeposit = deposit;
        StrategyId = strategyId;
    }
}