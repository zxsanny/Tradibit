namespace Tradibit.SharedUI.DTO.Primitives;

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