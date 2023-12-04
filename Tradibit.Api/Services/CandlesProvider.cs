using AutoMapper;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using CryptoExchange.Net.Sockets;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tradibit.DataAccess;
using Tradibit.Shared;
using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Coins;
using Tradibit.Shared.DTO.Primitives;
using Tradibit.Shared.DTO.Scenarios;
using Tradibit.Shared.Interfaces;
using SkenderQuote = Skender.Stock.Indicators.Quote;

namespace Tradibit.Api.Services;

public class CandlesProvider : ICandlesProvider,
    INotificationHandler<AppInitEvent>,
    IRequestHandler<ReplyHistoryEvent>
{
    private static readonly QuoteIndicators _quotes = new();
    private static readonly QuoteIndicators _historyQuotes = new();
    
    private readonly ILogger<CandlesProvider> _logger;
    private readonly IClientHolder _clientHolder;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly TradibitDb _db;
    private int _subscription;
    
    public CandlesProvider(ILogger<CandlesProvider> logger,
        IClientHolder clientHolder,
        IMediator mediator, 
        IMapper mapper,
        TradibitDb db)
    {
        _logger = logger;
        _clientHolder = clientHolder;
        _mediator = mediator;
        _mapper = mapper;
        _db = db;
    }
    
    //On the AppInitEvent, for the most cap coins for default intervals: 
    //1. Fill the _quotes and indicators
    //2. Subscribe on any KLine updates to constantly update _quotes and indicators
    public async Task Handle(AppInitEvent appInitEvent, CancellationToken cancellationToken)
    {
        var pairs = await _mediator.Send(new GetMostCapCoinsRequest(), cancellationToken);
        var defaultPainIntervals = pairs.ToPainIntervals(Constants.DefaultIntervals);
        var existingPairIntervals = await _db.Scenarios
            .Where(x => x.Status == ScenarioStatus.Running)
            .Select(x => x.PairInterval)
            .Distinct().ToListAsync(cancellationToken);
        var pairIntervals = defaultPainIntervals.Concat(existingPairIntervals).Distinct().ToList();
        
        foreach (var pairInterval in pairIntervals) 
            await _quotes.AddPairInterval(_clientHolder.MainClient, pairInterval, null, cancellationToken);
        
        //TODO: group by pair, and subscribe only on lowest interval for the pair, and update all the intervals for one pair
        var res = await _clientHolder.MainSocketClient.SpotStreams.SubscribeToKlineUpdatesAsync(pairs.Select(x => x.ToString()), 
            pairIntervals.Select(i => _mapper.Map<KlineInterval>(i)), OnMessage, cancellationToken);
        _subscription = res.Data.Id;
    }
    
    private void OnMessage(DataEvent<IBinanceStreamKlineData> msg)
    {
        var pairInterval = new PairInterval(Pair.Parse(msg.Data.Symbol), msg.Data.Data.Interval.ToInterval());
        var quoteIndicator = _quotes.Update(msg.Data.Data.ToQuote(), pairInterval);
        _mediator.Send(new KlineUpdateEvent(pairInterval, quoteIndicator));
    }
    
    public async Task<Unit> Handle(ReplyHistoryEvent e, CancellationToken cancellationToken)
    {
        foreach (var pairInterval in e.PairIntervals) 
            await _historyQuotes.AddPairInterval(_clientHolder.MainClient, pairInterval, e.HistorySpan, cancellationToken);

        var historyQuotes = _historyQuotes.GetHistory(e.PairIntervals);
        
        foreach (var historyQuote in historyQuotes)
            foreach (var pairIntervalQuote in historyQuote)
                await _mediator.Send(new KlineHistoryUpdateEvent(e.BackTestRunId, pairIntervalQuote.Key, pairIntervalQuote.Value), cancellationToken);
        
        return Unit.Value;
    }
    
    public decimal BtcValue => 
        _quotes.LastQuote(new PairInterval(Pair.BTC_USDT, Interval.I_15_MIN))?.Close ?? 0;
}