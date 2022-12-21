using MediatR;
using Tradibit.SharedUI.Primitives;

namespace Tradibit.Common.DTO.Events;

public class KlineUpdateEvent : IRequest<Unit>
{
    public Pair Pair { get; set; }
    public QuoteIndicator QuoteIndicator { get; set; }
    
    public KlineUpdateEvent(Pair pair, QuoteIndicator quoteIndicator)
    {
        Pair = pair;
        QuoteIndicator = quoteIndicator;
    }
}

public class KlineHistoryUpdateEvent : KlineUpdateEvent
{
    public Guid ScenarioId { get; set; }

    public KlineHistoryUpdateEvent(Guid scenarioId, KlineUpdateEvent klineUpdateEvent) : base(klineUpdateEvent.Pair, klineUpdateEvent.QuoteIndicator)
    {
        ScenarioId = scenarioId;
    }
}