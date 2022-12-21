using Binance.Net.Interfaces;
using Skender.Stock.Indicators;

namespace Tradibit.Common.Extensions;

public static class BinanceExtensions
{
    public static Quote ToQuote(this IBinanceStreamKline kline) =>
        new()
        {
            Date = kline.OpenTime,
            Open = kline.OpenPrice,
            High = kline.HighPrice,
            Low = kline.LowPrice,
            Close = kline.ClosePrice,
            Volume = kline.Volume
        };
    
    public static Quote ToQuote(this IBinanceKline kline) =>
        new()
        {
            Date = kline.OpenTime,
            Open = kline.OpenPrice,
            High = kline.HighPrice,
            Low = kline.LowPrice,
            Close = kline.ClosePrice,
            Volume = kline.Volume
        };
    
    public static Quote Update(this Quote quote, Quote update)
    {
        quote.Date = update.Date;
        quote.Close = update.Open;
        quote.High = update.High;
        quote.Low = update.Low;
        quote.Close = update.Close;
        quote.Volume = update.Volume;
        return quote;
    }
}