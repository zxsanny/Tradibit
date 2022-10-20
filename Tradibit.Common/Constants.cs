using Binance.Net.Enums;

namespace Tradibit.Common;

public static class Constants
{
    public static List<string> ExcludedCurrencies = new() { "BUSD", "USDC", "LUNA" };
    public static string AesKey = $"Tradibit{nameof(AesKey)}";
    public static string AesIv = $"Tradibit{nameof(AesIv)}";
    
    public static List<KlineInterval> DefaultIntervals = new() { KlineInterval.FifteenMinutes, KlineInterval.OneHour };
}