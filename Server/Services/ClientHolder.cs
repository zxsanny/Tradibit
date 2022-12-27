using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using Binance.Net.Clients;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Options;
using Tradibit.DataAccess;
using Tradibit.Shared;
using Tradibit.Shared.Entities;
using Tradibit.SharedUI.DTO.SettingsDTO;

namespace Tradibit.Api.Services;

public class ClientHolder : IClientHolder
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    private readonly ConcurrentDictionary<Guid, BinanceClient> _clients = new();
    private readonly ConcurrentDictionary<Guid, BinanceSocketClient> _socketClients = new();

    public BinanceClient MainClient { get; } 
    public BinanceSocketClient MainSocketClient { get; }
    
    public ConcurrentDictionary<Guid, int> Subscriptions { get; } = new();
    public BlockingCollection<(Guid UserId, DataEvent<BinanceStreamTrade> Event)> BinanceTrades { get; } = new();

    public ClientHolder(IServiceScopeFactory scopeFactory, IOptions<BinanceWatcherCredentials> binanceWatcherCredentials)
    {
        _scopeFactory = scopeFactory;
        
        var binanceCreds = binanceWatcherCredentials.Value;
        var apiCredentials = new ApiCredentials(
            EncryptionService.Decrypt(binanceCreds.BinanceKeyHash),
            EncryptionService.Decrypt(binanceCreds.BinanceSecretHash));

        MainClient = new BinanceClient(new BinanceClientOptions { ApiCredentials = apiCredentials });
        MainSocketClient = new BinanceSocketClient(new BinanceSocketClientOptions { ApiCredentials = apiCredentials });
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
        await GetClientGeneral(_clients, userId, user =>
        {
            var apiCredentials = new ApiCredentials(user.BinanceKey, user.BinanceSecret);
            return new BinanceClient(new BinanceClientOptions {ApiCredentials = apiCredentials});
        }, cancellationToken);
    
    public async Task<BinanceSocketClient> GetSocketClient(Guid userId, CancellationToken cancellationToken = default) => 
        await GetClientGeneral(_socketClients, userId, user =>
        {
            var apiCredentials = new ApiCredentials(user.BinanceKey, user.BinanceSecret);
            return new BinanceSocketClient(new BinanceSocketClientOptions { ApiCredentials = apiCredentials });
        }, cancellationToken);

}