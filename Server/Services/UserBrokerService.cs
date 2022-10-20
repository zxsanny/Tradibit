using System.Collections.Concurrent;
using Binance.Net.Enums;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Sockets;
using Tradibit.Api.Services.Binance;
using Tradibit.Common.DTO;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services;

public class UserBrokerService : BaseBinanceStreamService, IUserBrokerService
{
    private readonly ICoinsService _coinsService;
    
    public UserBrokerService(ILogger<UserBrokerService> logger, ICurrentUserProvider currentUserProvider, ICoinsService coinsService)
        : base(logger, currentUserProvider)
    {
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

    #region Trade stream Subscriptions
    
    protected override async Task<List<int>> LoginHandle(CancellationToken cancellationToken = default)
    {
        var pairs = (await _coinsService.GetMostCapitalisedPairs(cancellationToken))
            .Select(x => x.ToString());
        var res = await SocketClient.SpotStreams.SubscribeToTradeUpdatesAsync(pairs, OnTradeUpdate, cancellationToken);
        return new List<int> { res.Data.Id };
    }

    private void OnTradeUpdate(DataEvent<BinanceStreamTrade> obj)
    {
        _binanceTrades.TryAdd(obj);
    }
    
    #endregion
}