using MediatR;
using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO.Scenarios;


public class StartBackTestStrategyEvent : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid StrategyId { get; set; }
    public decimal Deposit { get; set; }
    public TimeSpan HistorySpan { get; set; }
    public List<Pair>? Pairs { get; set; }
}