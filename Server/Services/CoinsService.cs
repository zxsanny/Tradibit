﻿using MediatR;
using Microsoft.Extensions.Options;
using Tradibit.Common;
using Tradibit.Common.DTO;
using Tradibit.Common.SettingsDTO;
using Tradibit.SharedUI.DTO.Coins;
using Tradibit.SharedUI.Primitives;

namespace Tradibit.Api.Services;

public class CoinsService : IRequestHandler<GetMostCapCoinsRequest, List<Pair>>
{
    private readonly MainTradingSettings _mainTradingSettings;
    private readonly IClientHolder _clientHolder;
    
    public CoinsService(ILogger<CoinsService> logger, IOptions<MainTradingSettings> mainTradingSettings, IClientHolder clientHolder)
    {
        _clientHolder = clientHolder;
        _mainTradingSettings = mainTradingSettings.Value;
    }
    
    //TODO: Take coin's volatility into an account
    public async Task<List<Pair>> Handle(GetMostCapCoinsRequest request, CancellationToken cancellationToken)
    {
        var client = await _clientHolder.GetClient(request.UserId, cancellationToken);  
        
        return (await client.SpotApi.ExchangeData.GetProductsAsync(cancellationToken)).Data
            .Where(x => x.QuoteAsset == Currency.USDT)
            .Where(x => !Constants.ExcludedCurrencies.Contains(x.BaseAsset))
            .OrderByDescending(x => x.CirculatingSupply * x.ClosePrice)
            .Take(_mainTradingSettings.NumberPairsProcess)
            .Select(x => new Pair(x.BaseAsset, x.QuoteAsset))
            .ToList();   
    }
}