namespace Tradibit.Common.Interfaces;

public interface IStockProvider
{
    Task RunTickerListener(CancellationToken cancellationToken = default);
}