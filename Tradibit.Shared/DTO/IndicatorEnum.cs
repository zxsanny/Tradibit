namespace Tradibit.Shared.DTO;

public enum IndicatorEnum
{
    None = 0,
    
    SMA_20 = 10,
    SMA_50 = 12,
    SMA_100 = 14,
    SMA_200 = 16,

    EMA_20 = 20,
    EMA_50 = 22,
    EMA_100 = 24,
    EMA_200 = 26,

    MACD_H = 30,
    RSI = 30,
    
    PARABOLIC_SAR = 40,
    
    BOLLINGER_LOWER = 50,
    BOLLINGER_UPPER = 52,
}