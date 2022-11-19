using System.Collections.Concurrent;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Sockets;
using MediatR;
using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services;

public class UserBrokerService : IUserBrokerService,
    INotificationHandler<UserLoginEvent>,
    INotificationHandler<UserLogoutEvent>
{
    private readonly ILogger<UserBrokerService> _logger;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICoinsService _coinsService;
    
    private readonly ConcurrentDictionary<Guid, BinanceClient> _clients = new();
    private readonly ConcurrentDictionary<Guid, BinanceSocketClient> _socketClients = new();
    private readonly ConcurrentDictionary<Guid, int> _subscriptions = new();

    private BinanceClient Client => _clients.GetOrAdd(_currentUserProvider.CurrentUser.Id, _ => _currentUserProvider.GetClient());

    private async Task SubscribeSocketClient(CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.CurrentUser.Id;
        if (_socketClients.ContainsKey(userId))
            return;
        
        var socketClient = _currentUserProvider.GetSocketClient();
        _socketClients.TryAdd(userId, socketClient);
        
        var pairs = (await _coinsService.GetMostCapitalisedPairs(cancellationToken))
            .Select(x => x.ToString());
        var res = await socketClient.SpotStreams.SubscribeToTradeUpdatesAsync(pairs, OnTradeUpdate, cancellationToken);
        _subscriptions.TryAdd(userId, res.Data.Id);
    }

    private async Task UnsubscribeSocketClient(CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.CurrentUser.Id;
        if (!_socketClients.TryGetValue(userId, out var socketClient))
            return;
        
        if (!_subscriptions.TryGetValue(userId, out var subId))
            return;

        await socketClient.UnsubscribeAsync(subId);
    }
    
    private void OnTradeUpdate(DataEvent<BinanceStreamTrade> obj)
    {
        _binanceTrades.TryAdd(obj);
    }
    

    public UserBrokerService(ILogger<UserBrokerService> logger, ICurrentUserProvider currentUserProvider, ICoinsService coinsService)
    {
        _logger = logger;
        _currentUserProvider = currentUserProvider;
        _coinsService = coinsService;
    }
    
    #region Simple BUY SELL operations

    private readonly BlockingCollection<DataEvent<BinanceStreamTrade>> _binanceTrades = new();
    
    public async Task<decimal> Buy(Pair pair, decimal amount, CancellationToken cancellationToken = default) => 
        await ExecuteOrder(pair, amount, OrderSide.Buy, cancellationToken);

    public async Task<decimal> Sell(Pair pair, decimal amount, CancellationToken cancellationToken = default) =>
        await ExecuteOrder(pair, amount, OrderSide.Sell, cancellationToken);
    
    private async Task<decimal> ExecuteOrder(Pair pair, decimal amount, OrderSide orderSide, CancellationToken cancellationToken = default)
    {
        await Client.SpotApi.Trading.PlaceOrderAsync(pair.ToString(), orderSide, SpotOrderType.Market, amount, ct: cancellationToken);
        
        foreach (var trade in _binanceTrades.GetConsumingEnumerable())
            if (trade.Data.Symbol == pair.ToString())
                return trade.Data.Quantity;
        
        throw new Exception("No event occured");
    }
    
    #endregion

    #region Balances

    public async Task<decimal> GetUsdtBalance(CancellationToken cancellationToken = default)
    {
        var result = await Client.SpotApi.Account.GetBalancesAsync(asset: Currency.USDT, ct: cancellationToken);
        return result.Data.FirstOrDefault()!.Total;
    }
    
    public async Task<Dictionary<Currency, decimal>> GetBalances(CancellationToken cancellationToken = default)
    {
        var result = await Client.SpotApi.Account.GetBalancesAsync( ct: cancellationToken);
        return result.Data.ToDictionary(x => new Currency(x.Asset), x => x.Total);
    }

    #endregion

    public async Task Handle(UserLoginEvent notification, CancellationToken cancellationToken)
    {
        await SubscribeSocketClient(cancellationToken);
    }

    public Task Handle(UserLogoutEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}