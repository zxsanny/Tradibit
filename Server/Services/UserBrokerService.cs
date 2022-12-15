using System.Collections.Concurrent;
using Binance.Net.Enums;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Sockets;
using MediatR;
using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.Interfaces;
using Tradibit.DataAccess;

namespace Tradibit.Api.Services;

public class UserBrokerService : IUserBrokerService,
    INotificationHandler<UserLoginEvent>,
    INotificationHandler<UserLogoutEvent>
{
    private readonly ILogger<UserBrokerService> _logger;
    private readonly ICoinsService _coinsService;
    private readonly TradibitDb _db;
    private readonly IClientHolder _clientHolder;

    public UserBrokerService(ILogger<UserBrokerService> logger, ICoinsService coinsService, TradibitDb db, IClientHolder clientHolder)
    {
        _logger = logger;
        _coinsService = coinsService;
        _db = db;
        _clientHolder = clientHolder;
    }
    
    #region Balances

    public async Task<decimal> GetUsdtBalance(Guid userId, CancellationToken cancellationToken = default)
    {
        var client = await _clientHolder.GetClient(userId, cancellationToken);
        var res = await client.SpotApi.Account.GetAccountInfoAsync(ct: cancellationToken);
        
        var result = await client.SpotApi.Account.GetBalancesAsync(asset: Currency.USDT, ct: cancellationToken);
        return result.Data.FirstOrDefault()!.Total;
    }
    
    public async Task<Dictionary<Currency, decimal>> GetBalances(Guid userId, CancellationToken cancellationToken = default)
    {
        var client = await _clientHolder.GetClient(userId, cancellationToken);
        var result = await client.SpotApi.Account.GetBalancesAsync( ct: cancellationToken);
        return result.Data.ToDictionary(x => new Currency(x.Asset), x => x.BtcValuation);
    }

    #endregion
    
    #region Simple BUY SELL operations
    
    public async Task<decimal> Buy(Guid userId, Pair pair, decimal amount, CancellationToken cancellationToken = default) => 
        await ExecuteOrder(userId, pair, amount, OrderSide.Buy, cancellationToken);

    public async Task<decimal> Sell(Guid userId, Pair pair, decimal amount, CancellationToken cancellationToken = default) =>
        await ExecuteOrder(userId, pair, amount, OrderSide.Sell, cancellationToken);
    
    private async Task<decimal> ExecuteOrder(Guid userId, Pair pair, decimal amount, OrderSide orderSide, CancellationToken cancellationToken = default)
    {
        var client = await _clientHolder.GetClient(userId, cancellationToken);
        await client.SpotApi.Trading.PlaceOrderAsync(pair.ToString(), orderSide, SpotOrderType.Market, amount, ct: cancellationToken);
        
        foreach (var trade in _clientHolder.BinanceTrades.GetConsumingEnumerable())
            if (trade.UserId == userId && trade.Event.Data.Symbol == pair.ToString())
                return trade.Event.Data.Quantity;
        
        throw new Exception("No event occured");
    }
    
    #endregion

    //Subscribe to Trade updates in order to catch successful BUY or SELL operation (it's async)
    public async Task Handle(UserLoginEvent loginEvent, CancellationToken cancellationToken)
    {
        var socketClient = await _clientHolder.GetSocketClient(loginEvent.UserId, cancellationToken);
        var pairs = (await _coinsService.GetMostCapitalisedPairs(cancellationToken))
            .Select(x => x.ToString());
        var res = await socketClient.SpotStreams.SubscribeToTradeUpdatesAsync(pairs,
            tradeEvent => OnTradeUpdate(loginEvent.UserId, tradeEvent), cancellationToken);
        _clientHolder.Subscriptions.TryAdd(loginEvent.UserId, res.Data.Id);
    }
    
    private void OnTradeUpdate(Guid userId, DataEvent<BinanceStreamTrade> obj)
    {
        _clientHolder.BinanceTrades.TryAdd((userId, obj));
    }
    
    
    private async Task UnsubscribeSocketClient(Guid userId, CancellationToken cancellationToken)
    {
        var socketClient = await _clientHolder.GetSocketClient(userId, cancellationToken);
        if (!_clientHolder.Subscriptions.TryGetValue(userId, out var subId))
            return;
        await socketClient.UnsubscribeAsync(subId);
    }
    
    public async Task Handle(UserLogoutEvent userLogoutEvent, CancellationToken cancellationToken)
    {
        //await UnsubscribeSocketClient(userLogoutEvent.UserId, cancellationToken);
    }
}