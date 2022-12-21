using Binance.Net.Enums;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Sockets;
using MediatR;
using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.DTO.Events.UserBroker;
using Tradibit.DataAccess;
using Tradibit.SharedUI.DTO.Coins;
using Tradibit.SharedUI.Primitives;
using OrderSide = Binance.Net.Enums.OrderSide;

namespace Tradibit.Api.Services;

public class UserBrokerService : 
    INotificationHandler<UserLoginEvent>,
    INotificationHandler<UserLogoutEvent>,
    IRequestHandler<GetBalancesEvent, Dictionary<Currency, decimal>>,
    IRequestHandler<BuyEvent, decimal>,
    IRequestHandler<SellEvent, decimal>
{
    private readonly ILogger<UserBrokerService> _logger;
    private readonly IClientHolder _clientHolder;
    private readonly IMediator _mediator;

    public UserBrokerService(ILogger<UserBrokerService> logger, TradibitDb db, IClientHolder clientHolder, IMediator mediator)
    {
        _logger = logger;
        _clientHolder = clientHolder;
        _mediator = mediator;
    }
    
    public async Task<Dictionary<Currency, decimal>> Handle(GetBalancesEvent request, CancellationToken cancellationToken)
    {
        var client = await _clientHolder.GetClient(request.UserId, cancellationToken);
        var result = await client.SpotApi.Account.GetBalancesAsync(asset: request.Asset, ct: cancellationToken);
        return result.Data.ToDictionary(x => new Currency(x.Asset), x => x.BtcValuation);
    }
    
    #region Simple BUY SELL operations
    
    public async Task<decimal> Handle(BuyEvent request, CancellationToken cancellationToken) => 
        await ExecuteOrder(request, OrderSide.Buy, cancellationToken);

    public async Task<decimal> Handle(SellEvent request, CancellationToken cancellationToken) =>
        await ExecuteOrder(request, OrderSide.Sell, cancellationToken);
    
    private async Task<decimal> ExecuteOrder(BaseOrderEvent e, OrderSide orderSide, CancellationToken cancellationToken = default)
    {
        var client = await _clientHolder.GetClient(e.UserId, cancellationToken);
        await client.SpotApi.Trading.PlaceOrderAsync(e.Pair.ToString(), orderSide, SpotOrderType.Market, e.Amount, ct: cancellationToken);
        
        foreach (var trade in _clientHolder.BinanceTrades.GetConsumingEnumerable())
            if (trade.UserId == e.UserId && trade.Event.Data.Symbol == e.Pair.ToString())
                return trade.Event.Data.Quantity;
        
        throw new Exception("No event occured");
    }
    
    #endregion

    //Subscribe to Trade updates in order to catch successful BUY or SELL operation (it's async)
    public async Task Handle(UserLoginEvent loginEvent, CancellationToken cancellationToken)
    {
        var socketClient = await _clientHolder.GetSocketClient(loginEvent.UserId, cancellationToken);
        var pairs = (await _mediator.Send(new GetMostCapCoinsRequest(loginEvent.UserId), cancellationToken))
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