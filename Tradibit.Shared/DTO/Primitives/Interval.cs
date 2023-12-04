using Binance.Net.Enums;

namespace Tradibit.Shared.DTO.Primitives;

public enum Interval
{
    I_1_SEC = 1,
    I_1_MIN = 60, 
    I_3_MIN = 60 * 3, 
    I_5_MIN = 60 * 5,
    I_15_MIN = 60 * 15,
    I_30_MIN = 60 * 30, 
    I_1_HOUR = 60 * 60,
    I_2_HOUR = 60 * 60 * 2,
    I_4_HOUR = 60 * 60 * 4,
    I_6_HOUR = 60 * 60 * 6,
    I_8_HOUR = 60 * 60 * 8,
    I_12_HOUR = 60 * 60 * 12,
    I_1_DAY = 60 * 60 * 24,
    I_3_DAY = 60 * 60 * 24 * 3,
    I_1_WEEK = 60 * 60 * 24 * 7,
    I_1_MONTH = 60 * 60 * 24 * 30
}

public static class IntervalExtensions
{
    public static KlineInterval ToKlineInterval(this Interval interval) =>
        interval switch
        {
            Interval.I_1_SEC  => KlineInterval.OneSecond,
            Interval.I_1_MIN => KlineInterval.OneMinute,
            Interval.I_3_MIN => KlineInterval.ThreeMinutes,
            Interval.I_5_MIN => KlineInterval.FiveMinutes,
            Interval.I_15_MIN => KlineInterval.FifteenMinutes,
            Interval.I_30_MIN => KlineInterval.ThirtyMinutes,
            Interval.I_1_HOUR => KlineInterval.OneHour,
            Interval.I_2_HOUR => KlineInterval.TwoHour,
            Interval.I_4_HOUR => KlineInterval.FourHour,
            Interval.I_6_HOUR => KlineInterval.SixHour,
            Interval.I_8_HOUR => KlineInterval.EightHour,
            Interval.I_12_HOUR => KlineInterval.TwelveHour,
            Interval.I_1_DAY => KlineInterval.OneDay,
            Interval.I_3_DAY => KlineInterval.ThreeDay,
            Interval.I_1_WEEK => KlineInterval.OneWeek,
            Interval.I_1_MONTH => KlineInterval.OneMonth,
            _ => KlineInterval.OneHour
        };
    
    public static Interval ToInterval(this KlineInterval interval) =>
        interval switch
        {
            KlineInterval.OneSecond => Interval.I_1_SEC ,
            KlineInterval.OneMinute => Interval.I_1_MIN,
            KlineInterval.ThreeMinutes => Interval.I_3_MIN,
            KlineInterval.FiveMinutes => Interval.I_5_MIN,
            KlineInterval.FifteenMinutes => Interval.I_15_MIN,
            KlineInterval.ThirtyMinutes => Interval.I_30_MIN,
            KlineInterval.OneHour => Interval.I_1_HOUR,
            KlineInterval.TwoHour => Interval.I_2_HOUR,
            KlineInterval.FourHour => Interval.I_4_HOUR,
            KlineInterval.SixHour => Interval.I_6_HOUR,
            KlineInterval.EightHour => Interval.I_8_HOUR,
            KlineInterval.TwelveHour => Interval.I_12_HOUR,
            KlineInterval.OneDay => Interval.I_1_DAY,
            KlineInterval.ThreeDay => Interval.I_3_DAY,
            KlineInterval.OneWeek => Interval.I_1_WEEK,
            KlineInterval.OneMonth => Interval.I_1_MONTH,
            _ => Interval.I_1_HOUR
        };
}