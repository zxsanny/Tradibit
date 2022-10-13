using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Options;
using Skender.Stock.Indicators;
using Tradibit.Common.DTO;
using Tradibit.Common.Extensions;

namespace Tradibit.Api.Providers;

public class RealtimeCandlesProvider : ICandlesProvider
{
    private readonly ILogger<RealtimeCandlesProvider> _logger;
    private readonly MainTradingSettings _mainTradingSettings;
    private readonly List<string> _excludedCurrencies = new() { "BUSD", "USDC", "LUNA" };
    private readonly List<KlineInterval> _intervals = new() { KlineInterval.FifteenMinutes, KlineInterval.OneHour }; 
    
    private readonly BinanceClient _client;
    private readonly BinanceSocketClient _socketClient;

    public Dictionary<PairIntervalKey, List<Quote>> Quotes { get; } = new();
    public Dictionary<PairIntervalKey, Dictionary<IndicatorEnum, List<double?>>> Indicators { get; } = new();
    
    private Action<Pair, Quote, Dictionary<IndicatorEnum, double?>> _handler;
    
    public RealtimeCandlesProvider(ILogger<RealtimeCandlesProvider> logger, IOptions<MainTradingSettings> mainTradingSettings)
    {
        _logger = logger;
        _mainTradingSettings = mainTradingSettings.Value;
        
        //TODO: move this to the secure storage
        var credentials = new ApiCredentials(
            "rUgJWN8pMcIaBKJX40yFxixq6YRk89KQgyg64LRSY2cGn2FuhrkOZg4KyB0rjodZ".ToSecureString(),
            "SAElwqvy1oqXRyvfTHavG6YsAvMZO9B0CUTXNpLXkiWv030pqna3iG8XUBgTzaCf".ToSecureString());
        
        _client = new BinanceClient(new BinanceClientOptions { ApiCredentials = credentials });
        _socketClient = new(new BinanceSocketClientOptions { ApiCredentials = credentials, LogWriters = new List<ILogger> { _logger} });
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        //TODO: Take coin's volatility into an account
        var pairs = await GetMostCapitalisedPairs(_mainTradingSettings.NumberPairsProcess, cancellationToken);
        foreach (var pair in pairs)
        {
            foreach (var interval in _intervals)
            {
                var candlesKey = new PairIntervalKey(pair, interval);
                Quotes[candlesKey] = (await _client.SpotApi.ExchangeData.GetKlinesAsync(pair.ToString(), interval, ct: cancellationToken))
                    .Data.Select(x => x.ToQuote()).ToList();
                SetIndicators(candlesKey);
            }
        }

        await _socketClient.SpotStreams.SubscribeToKlineUpdatesAsync(pairs.Select(x => x.ToString()), _intervals, OnMessage, cancellationToken);
    }

    private void OnMessage(DataEvent<IBinanceStreamKlineData> msg)
    {
        var quote = msg.Data.Data.ToQuote();
        var pair = Pair.ParseOrDefault(msg.Data.Symbol);
        var candlesKey = new PairIntervalKey(pair, msg.Data.Data.Interval);
        var quotes = Quotes[candlesKey];

        if (quotes[^1].Date == quote.Date) 
            quotes[^1].Update(quote);
        else
            quotes.Add(quote);
        SetIndicators(candlesKey);

        var indicators = Indicators[candlesKey].ToDictionary(x => x.Key, x => x.Value.LastOrDefault());
        
        if (_handler != null)
            _handler(pair, quotes[^1], indicators);
    }
    
    public async Task Subscribe(Action<Pair, Quote, Dictionary<IndicatorEnum, double?>> handler)
    {
        _handler = handler;
    }
    
    private async Task<List<Pair>> GetMostCapitalisedPairs(int count, CancellationToken cancellationToken = default) =>
        (await _client.SpotApi.ExchangeData.GetProductsAsync(cancellationToken)).Data
        .Where(x => x.QuoteAsset == Currency.USDT)
        .Where(x => !_excludedCurrencies.Contains(x.BaseAsset))
        .OrderByDescending(x => x.CirculatingSupply * x.ClosePrice)
        .Take(count).Select(x => new Pair(x.BaseAsset, x.QuoteAsset)).ToList();
    
    private void SetIndicators(PairIntervalKey pairIntervalKey)
    {
        var quotes = Quotes[pairIntervalKey];
        Indicators[pairIntervalKey][IndicatorEnum.SMA_20] = quotes.GetSma(20).Select(x => x.Sma).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.SMA_50] = quotes.GetSma(50).Select(x => x.Sma).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.SMA_100] = quotes.GetSma(100).Select(x => x.Sma).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.SMA_200] = quotes.GetSma(200).Select(x => x.Sma).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.EMA_20] = quotes.GetEma(20).Select(x => x.Ema).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.EMA_50] = quotes.GetEma(50).Select(x => x.Ema).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.EMA_100] = quotes.GetEma(100).Select(x => x.Ema).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.EMA_200] = quotes.GetEma(200).Select(x => x.Ema).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.MACD_H] = quotes.GetMacd().Select(x => x.Histogram).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.RSI] = quotes.GetRsi().Select(x => x.Rsi).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.PARABOLIC_SAR] = quotes.GetParabolicSar().Select(x => x.Sar).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.BOLLINGER_UPPER] = quotes.GetBollingerBands().Select(x => x.UpperBand).ToList();
        Indicators[pairIntervalKey][IndicatorEnum.BOLLINGER_LOWER] = quotes.GetBollingerBands().Select(x => x.LowerBand).ToList();        
    }
}
