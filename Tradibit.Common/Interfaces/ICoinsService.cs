using Tradibit.Common.DTO;

namespace Tradibit.Common.Interfaces;

public interface ICoinsService
{
    Task<List<Pair>> GetMostCapitalisedPairs(CancellationToken cancellationToken = default);
}