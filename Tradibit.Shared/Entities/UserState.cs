using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.Shared.Entities;

public class UserState : BaseTrackableId
{
    public List<(Pair, decimal)> ActivePairs { get; set; }
    public decimal TotalDeposit { get; set; }
    public decimal CurrentDeposit { get; set; }
}