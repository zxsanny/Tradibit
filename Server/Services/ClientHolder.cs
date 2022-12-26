using System.Collections.Concurrent;
using Binance.Net.Clients;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Sockets;
using Tradibit.DataAccess;
using Tradibit.Shared.Entities;

namespace Tradibit.Api.Services;

public class ClientHolder : IClientHolder
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<Guid, BinanceClient> _clients = new();
    private readonly ConcurrentDictionary<Guid, BinanceSocketClient> _socketClients = new();

    public ConcurrentDictionary<Guid, int> Subscriptions { get; } = new();
    public BlockingCollection<(Guid UserId, DataEvent<BinanceStreamTrade> Event)> BinanceTrades { get; } = new();

    public ClientHolder(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    private async Task<TClient> GetClientGeneral<TClient>(ConcurrentDictionary<Guid, TClient> clientsStore, Guid userId, 
        Func<User, TClient> getClientFunc, CancellationToken cancellationToken = default)
    {
        if (clientsStore.TryGetValue(userId, out var client))
            return client;

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TradibitDb>();
        var user = await db.Users.FindAsync(userId, cancellationToken); 
        
        client = getClientFunc(user);
        clientsStore.TryAdd(userId, client);
        return client;
    }

    public async Task<BinanceClient> GetClient(Guid userId, CancellationToken cancellationToken = default) => 
        await GetClientGeneral(_clients, userId, user => user.GetClient(), cancellationToken);
    
    public async Task<BinanceSocketClient> GetSocketClient(Guid userId, CancellationToken cancellationToken = default) => 
        await GetClientGeneral(_socketClients, userId, user => user.GetSocketClient(), cancellationToken);

}