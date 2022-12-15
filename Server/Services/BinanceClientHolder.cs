using System.Collections.Concurrent;
using Binance.Net.Clients;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Sockets;
using Tradibit.Common.Entities;
using Tradibit.DataAccess;

namespace Tradibit.Api.Services;

public interface IClientHolder
{
    Task<BinanceClient> GetClient(Guid userId, CancellationToken cancellationToken = default);
    Task<BinanceSocketClient> GetSocketClient(Guid userId, CancellationToken cancellationToken = default);
    BlockingCollection<(Guid UserId, DataEvent<BinanceStreamTrade> Event)> BinanceTrades { get; }
    ConcurrentDictionary<Guid, int> Subscriptions { get; }
}

public class ClientHolder : IClientHolder
{
    private readonly TradibitDb _db;
    private readonly ConcurrentDictionary<Guid, BinanceClient> _clients = new();
    private readonly ConcurrentDictionary<Guid, BinanceSocketClient> _socketClients = new();

    public ConcurrentDictionary<Guid, int> Subscriptions { get; } = new();
    public BlockingCollection<(Guid UserId, DataEvent<BinanceStreamTrade> Event)> BinanceTrades { get; } = new();

    public ClientHolder(TradibitDb db)
    {
        _db = db;
    }
    
    private async Task<TClient> GetClientGeneral<TClient>(ConcurrentDictionary<Guid, TClient> clientsStore, Guid userId, 
        Func<User, TClient> getClientFunc, CancellationToken cancellationToken = default)
    {
        if (clientsStore.TryGetValue(userId, out var client))
            return client;
        var user = await _db.Users.FindAsync(userId, cancellationToken); 
        client = getClientFunc(user);
        
        clientsStore.TryAdd(userId, client);
        return client;
    }

    public async Task<BinanceClient> GetClient(Guid userId, CancellationToken cancellationToken = default) => 
        await GetClientGeneral(_clients, userId, user => user.GetClient(), cancellationToken);
    
    public async Task<BinanceSocketClient> GetSocketClient(Guid userId, CancellationToken cancellationToken = default) => 
        await GetClientGeneral(_socketClients, userId, user => user.GetSocketClient(), cancellationToken);

}