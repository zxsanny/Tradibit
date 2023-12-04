using Tradibit.Shared.DTO.Primitives;
using Tradibit.Shared.Entities;

namespace Tradibit.Shared;

public static class Constants
{
    public static readonly List<string> ExcludedCurrencies = new() { "BUSD", "USDC", "LUNA" };
    public static string AesKey = $"Tradibit{nameof(AesKey)}";
    public static string AesIv = $"Tradibit{nameof(AesIv)}";
 
    /// <summary>Name of key to store authentication token in LocalStorage</summary>
    public const string AUTH_TOKEN_KEY = nameof(AUTH_TOKEN_KEY);

    public const string APP_AUTH_SCHEME = "Application";
    
    public static readonly List<Interval> DefaultIntervals = new()
    {
        Interval.I_5_MIN,
        Interval.I_1_HOUR
    };
}