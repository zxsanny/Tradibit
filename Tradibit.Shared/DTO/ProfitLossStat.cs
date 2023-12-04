namespace Tradibit.Shared.DTO;

public class ProfitLossStat
{
    public decimal TotalProfitLossPercent { get; set; }
    public decimal TotalProfitLoss { get; set; }
    public TimeSpan TotalActiveTime { get; set; }

    public ProfitLossStat(decimal totalProfitLossPercent, decimal totalProfitLoss, TimeSpan totalActiveTime)
    {
        TotalProfitLossPercent = totalProfitLossPercent;
        TotalProfitLoss = totalProfitLoss;
        TotalActiveTime = totalActiveTime;
    }

    public ProfitLossStat(DateTime? startTime, decimal? start, decimal? now)
    {
        if (!startTime.HasValue || !start.HasValue || !now.HasValue)
        {
            TotalProfitLoss = TotalProfitLossPercent = 0;
            TotalActiveTime = TimeSpan.Zero;
            return;
        }
        
        TotalProfitLoss = now.Value - start.Value;
        TotalProfitLossPercent = TotalProfitLoss * 100 / now.Value;
        TotalActiveTime = DateTime.UtcNow.Subtract(startTime.Value);
    }
}