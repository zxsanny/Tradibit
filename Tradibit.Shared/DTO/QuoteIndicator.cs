using Skender.Stock.Indicators;

namespace Tradibit.Common.DTO;

public class QuoteIndicator
{
    public Quote Quote { get; set; } 
    public Dictionary<IndicatorEnum, decimal?> Indicators { get; set; }

    public QuoteIndicator(Quote quote, Dictionary<IndicatorEnum, decimal?> indicators)
    {
        Quote = quote;
        Indicators = indicators;
    }
}