using MediatR;
using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.DTO;

public class KlineUpdateEvent : IRequest<Unit>
{
    public PairInterval PairInterval { get; set; }
    public QuoteIndicator QuoteIndicator { get; set; }
    
    public KlineUpdateEvent(PairInterval pairInterval, QuoteIndicator quoteIndicator)
    {
        PairInterval = pairInterval;
        QuoteIndicator = quoteIndicator;
    }
}

public class KlineHistoryUpdateEvent : KlineUpdateEvent
{
    public Guid BackTestRunId { get; set; }

    public KlineHistoryUpdateEvent(Guid backTestRunId, PairInterval pairInterval, QuoteIndicator quoteIndicator) 
        : base(pairInterval, quoteIndicator)
    {
        BackTestRunId = backTestRunId;
    }
}
