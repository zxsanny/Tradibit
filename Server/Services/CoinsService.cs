using Microsoft.Extensions.Options;
using Tradibit.Api.Services.Binance;
using Tradibit.Common;
using Tradibit.Common.DTO;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services;

public class CoinsService : BaseBinanceService, ICoinsService
{
    private readonly MainTradingSettings _mainTradingSettings;

    public CoinsService(ILogger<CoinsService> logger, ICurrentUserProvider currentUserProvider, IOptions<MainTradingSettings> mainTradingSettings)
        : base(logger, currentUserProvider)
    {
        _mainTradingSettings = mainTradingSettings.Value;
    }
    
    //TODO: Take coin's volatility into an account
    public async Task<List<Pair>> GetMostCapitalisedPairs(CancellationToken cancellationToken = default) =>
        (await Client.SpotApi.ExchangeData.GetProductsAsync(cancellationToken)).Data
        .Where(x => x.QuoteAsset == Currency.USDT)
        .Where(x => !Constants.ExcludedCurrencies.Contains(x.BaseAsset))
        .OrderByDescending(x => x.CirculatingSupply * x.ClosePrice)
        .Take(_mainTradingSettings.NumberPairsProcess)
        .Select(x => new Pair(x.BaseAsset, x.QuoteAsset)).ToList();
}