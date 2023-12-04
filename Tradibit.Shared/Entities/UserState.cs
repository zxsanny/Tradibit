using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Primitives;
using Tradibit.SharedUI.DTO;

namespace Tradibit.Shared.Entities;

public class UserState : BaseTrackableId
{
    //to json
    public List<ActivePair> ActivePairs { get; set; }
    public decimal CurrentDeposit { get; set; }
}

public class ActivePair
{
    public PairInterval PairInterval { get; set; }
    public decimal Amount { get; set; }

    //for ef-core
    public ActivePair(){}
    
    public ActivePair(PairInterval pairInterval, decimal amount)
    {
        PairInterval = pairInterval;
        Amount = amount;
    }
}