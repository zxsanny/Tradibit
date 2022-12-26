using Binance.Net.Clients;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Skender.Stock.Indicators;
using Tradibit.Shared.Entities;

namespace Tradibit.Api.Services;

public static class BinanceExtensions
{
    public static BinanceClient GetClient(this User user)
    {
        var q = new Quote();
        var apiCredentials = new ApiCredentials(user.BinanceKey, user.BinanceSecret);
        return new BinanceClient(new BinanceClientOptions {ApiCredentials = apiCredentials});
    }
    
    public static BinanceSocketClient GetSocketClient(this User user)
    {
        var apiCredentials = new ApiCredentials(user.BinanceKey, user.BinanceSecret);
        return new BinanceSocketClient(new BinanceSocketClientOptions { ApiCredentials = apiCredentials });
    }
}