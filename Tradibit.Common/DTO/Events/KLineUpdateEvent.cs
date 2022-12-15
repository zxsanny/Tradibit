using System.Runtime;
using MediatR;
using Skender.Stock.Indicators;

namespace Tradibit.Common.DTO.Events;

public class KlineUpdateEvent : IRequest<Unit>
{
    public Pair Pair { get; set; }
    public Quote Quote { get; set; } 
    public Dictionary<IndicatorEnum, decimal?> Indicators { get; set; }
    public bool IsHistory { get; set; }
    
    public KlineUpdateEvent(Pair pair, Quote quote, Dictionary<IndicatorEnum, decimal?> indicators, bool isHistory = false)
    {
        Pair = pair;
        Quote = quote;
        Indicators = indicators;
        IsHistory = isHistory;
    }
}

public class HistoryKlineUpdateEvent : KlineUpdateEvent
{
    public Guid ScenarioId { get; set; }
    public Guid UserId { get; set; }
    
    public HistoryKlineUpdateEvent(Guid scenarioId, Guid userId, Pair pair, Quote quote, Dictionary<IndicatorEnum, decimal?> indicators, bool isHistory = false)
        : base(pair, quote, indicators, isHistory)
    {
        ScenarioId = scenarioId;
        UserId = userId;
    }
}