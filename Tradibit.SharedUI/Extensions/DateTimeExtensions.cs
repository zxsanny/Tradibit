using System.Globalization;

namespace Tradibit.SharedUI.Extensions;

/// <summary></summary>
public static class DateTimeExtensions
{
    /// <summary> Converts unix timestamp to datetime </summary>
    public static DateTime FromUnixTimeStamp(this double unixTimeStamp ) =>
        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(unixTimeStamp).ToLocalTime();
        
    /// <summary> </summary>
    public static string ToISODateTime(this DateTime date) =>
        date.ToString("yyyy/MM/dd HH:mm:ss");
        
    /// <summary> </summary>
    public static string ToISODateTimeWithOffset(this DateTime date) =>
        date.ToString("yyyy/MM/dd HH:mm:ss (UTCz)");
        
    /// <summary>Returns string representation of date in yyyy-MM-dd format</summary>
    public static string ToISODate(this DateTime date) =>
        date.ToString("O")[..10];

    /// <summary>Returns string representing date for dynamo db. Need to use this in all queries working with date search </summary>
    public static string ToDynamo(this DateTime date) =>
        date.ToUniversalTime().ToString("O");

    /// <summary>Returns string representing date for dynamo db. Need to use this in all queries working with date search </summary>
    public static string ToDynamo(this DateTime? date) =>
        date.HasValue ? date.Value.ToDynamo() : "";
        
        
    /// <summary> </summary>
    public static DateTime? GetDateTime(this string s, string dateFormat = "dd/MM/yyyy")
    {
        if (string.IsNullOrEmpty(s))
            return null;

        if (!DateTime.TryParseExact(s, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            return null;

        return result;
    }
        
    /// <summary> </summary>
    public static DateTime? GetStartDateTime(this string s, string dateFormat = "dd/MM/yyyy")
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        if (!DateTime.TryParseExact(s, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            return null;

        result = result.Date;
        return result;
    }

    /// <summary> </summary>
    public static DateTime? GetEndDateTime(this string s, string dateFormat = "dd/MM/yyyy")
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        if (!DateTime.TryParseExact(s, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)) 
            return null;

        result = result.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        return result;
    }
}