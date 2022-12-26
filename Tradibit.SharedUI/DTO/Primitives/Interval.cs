namespace Tradibit.SharedUI.DTO.Primitives;

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