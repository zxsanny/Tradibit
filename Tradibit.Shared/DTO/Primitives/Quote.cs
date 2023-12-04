using Binance.Net.Interfaces;
using SkenderQuote = Skender.Stock.Indicators.Quote;

namespace Tradibit.Shared.DTO.Primitives;

public class Quote
{
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    public void Update(Quote quote)
    {
        Date = quote.Date;
        Open = quote.Open;
        High = quote.High;
        Low = quote.Low;
        Close = quote.Close;
        Volume = quote.Volume;
    }
}

public static class QuoteExtensions
{
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

    public static SkenderQuote ToSkenderQuote(this Quote quote) =>
        new()
        {
            Date = quote.Date,
            Open = quote.Open,
            High = quote.High,
            Low = quote.Low,
            Close = quote.Close,
            Volume = quote.Volume
        };
}