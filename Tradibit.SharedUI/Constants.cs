using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI;

public static class Constants
{
    public static readonly List<string> ExcludedCurrencies = new() { "BUSD", "USDC", "LUNA" };
    public static string AesKey = $"Tradibit{nameof(AesKey)}";
    public static string AesIv = $"Tradibit{nameof(AesIv)}";
 
    /// <summary>Name of key to store authentication token in LocalStorage</summary>
    public const string AUTH_TOKEN_KEY = nameof(AUTH_TOKEN_KEY);
    
    public static readonly List<Interval> DefaultIntervals = new()
    {
        Interval.I_15_MIN,
        Interval.I_2_HOUR
    };
}