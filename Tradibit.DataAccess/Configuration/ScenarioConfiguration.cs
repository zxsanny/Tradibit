using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Tradibit.Shared.Entities;

namespace Tradibit.DataAccess.Configuration;

public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
    public void Configure(EntityTypeBuilder<Scenario> builder)
    {
        builder.HasKey(sc => sc.Id);
        
        builder.Property(sc => sc.UserVars)
            .HasConversion(v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, decimal?>>(v) ?? new Dictionary<string, decimal?>())
            .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, decimal?>>(
                (p1, p2) => p1!.SequenceEqual(p2!),
                list => list.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                list => list));
        
        builder.OwnsOne(sc => sc.PairInterval, p =>
        {
            p.OwnsOne(x => x.Pair, pair =>
            {
                pair.OwnsOne(x => x.BaseCurrency);
                pair.Navigation(x => x.BaseCurrency).IsRequired();
                
                pair.OwnsOne(x => x.QuoteCurrency);
                pair.Navigation(x => x.QuoteCurrency).IsRequired();
            });
            p.Navigation(x => x.Pair).IsRequired();
        });
        builder.Navigation(c => c.PairInterval).IsRequired();
        
        builder.HasOne(x => x.Strategy)
            .WithMany(x => x.Scenarios)
            .HasForeignKey(x => x.StrategyId);
    }
}