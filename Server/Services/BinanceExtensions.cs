using Binance.Net.Clients;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services;

public static class BinanceExtensions
{
    public static BinanceClient GetClient(this ICurrentUserProvider currentUserProvider)
    {
        var user = currentUserProvider.CurrentUser;
        var apiCredentials = new ApiCredentials(user.BinanceKey, user.BinanceSecret);
        return new BinanceClient(new BinanceClientOptions {ApiCredentials = apiCredentials});
    }
    
    public static BinanceSocketClient GetSocketClient(this ICurrentUserProvider currentUserProvider)
    {
        var user = currentUserProvider.CurrentUser;
        var apiCredentials = new ApiCredentials(user.BinanceKey, user.BinanceSecret);
        return new BinanceSocketClient(new BinanceSocketClientOptions { ApiCredentials = apiCredentials });
    }
}