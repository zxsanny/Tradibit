using System.Collections.Concurrent;
using Binance.Net.Clients;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Sockets;

namespace Tradibit.Common.Interfaces;

public interface IClientHolder
{
    Task<BinanceClient> GetClient(Guid userId, CancellationToken cancellationToken = default);
    Task<BinanceSocketClient> GetSocketClient(Guid userId, CancellationToken cancellationToken = default);
    BlockingCollection<(Guid UserId, DataEvent<BinanceStreamTrade> Event)> BinanceTrades { get; }
    ConcurrentDictionary<Guid, int> Subscriptions { get; }
}