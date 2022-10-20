using Binance.Net.Clients;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services.Binance;

public class BaseBinanceService
{
    protected ILogger Logger { get; }
    protected ICurrentUserProvider CurrentUserProvider { get; }
    
    protected ApiCredentials ApiCredentials { get; }
    protected BinanceClient Client { get; }
    
    protected BaseBinanceService(ILogger logger, ICurrentUserProvider currentUserProvider)
    {
        Logger = logger;
        CurrentUserProvider = currentUserProvider;
        var user = currentUserProvider.CurrentUser;
        ApiCredentials = new ApiCredentials(user.BinanceKey, user.BinanceSecret);
        
        Client = new BinanceClient(new BinanceClientOptions { ApiCredentials = ApiCredentials });
    }
}