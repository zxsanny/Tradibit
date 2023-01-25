using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.Shared.Entities;

public class UserState : BaseTrackableId
{
    //to json
    public List<ActivePair> ActivePairs { get; set; }
    public decimal CurrentDeposit { get; set; }
}

public class ActivePair
{
    public Pair Pair { get; set; }
    public decimal Amount { get; set; }

    //for ef-core
    public ActivePair(){}
    
    public ActivePair(Pair pair, decimal amount)
    {
        Pair = pair;
        Amount = amount;
    }
}