using MediatR;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO;

public class KlineUpdateEvent : IRequest<Unit>
{
    public PairIntervalKey PairIntervalKey { get; set; }
    public QuoteIndicator QuoteIndicator { get; set; }
    
    public KlineUpdateEvent(PairIntervalKey pairIntervalKey, QuoteIndicator quoteIndicator)
    {
        PairIntervalKey = pairIntervalKey;
        QuoteIndicator = quoteIndicator;
    }
}

public class KlineHistoryUpdateEvent : KlineUpdateEvent
{
    public Guid StrategyId { get; set; }

    public KlineHistoryUpdateEvent(Guid strategyId, KlineUpdateEvent klineUpdateEvent) 
        : base(klineUpdateEvent.PairIntervalKey, klineUpdateEvent.QuoteIndicator)
    {
        StrategyId = strategyId;
    }
}