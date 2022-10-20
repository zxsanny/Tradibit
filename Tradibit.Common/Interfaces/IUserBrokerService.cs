using Tradibit.Common.DTO;

namespace Tradibit.Common.Interfaces;

public interface IUserBrokerService
{
    Task<decimal> GetUsdtBalance(CancellationToken cancellationToken = default);
    Task<Dictionary<Currency, decimal>> GetBalances(CancellationToken cancellationToken = default);
    
    Task<decimal> Buy(Pair pair, decimal amount, CancellationToken cancellationToken = default);
    Task<decimal> Sell(Pair pair, decimal amount, CancellationToken cancellationToken = default);
}