using Binance.Net.Clients;
using MediatR;
using Microsoft.Extensions.Options;
using Tradibit.Common;
using Tradibit.Common.DTO;
using Tradibit.Common.DTO.Events;
using Tradibit.Common.Interfaces;
using Tradibit.Common.SettingsDTO;

namespace Tradibit.Api.Services;

public class CoinsService : ICoinsService, INotificationHandler<UserLoginEvent>
{
    private readonly ILogger<CoinsService> _logger;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly MainTradingSettings _mainTradingSettings;
    private BinanceClient _client;
    
    public CoinsService(ILogger<CoinsService> logger, ICurrentUserProvider currentUserProvider, IOptions<MainTradingSettings> mainTradingSettings)
    {
        _logger = logger;
        _currentUserProvider = currentUserProvider;
        _mainTradingSettings = mainTradingSettings.Value;
        
    }

    public async Task Handle(UserLoginEvent notification, CancellationToken cancellationToken)
    {
        _client ??= _currentUserProvider.GetClient();
    }

    //TODO: Take coin's volatility into an account
    public async Task<List<Pair>> GetMostCapitalisedPairs(CancellationToken cancellationToken = default) =>
        (await _client.SpotApi.ExchangeData.GetProductsAsync(cancellationToken)).Data
        .Where(x => x.QuoteAsset == Currency.USDT)
        .Where(x => !Constants.ExcludedCurrencies.Contains(x.BaseAsset))
        .OrderByDescending(x => x.CirculatingSupply * x.ClosePrice)
        .Take(_mainTradingSettings.NumberPairsProcess)
        .Select(x => new Pair(x.BaseAsset, x.QuoteAsset)).ToList();
}