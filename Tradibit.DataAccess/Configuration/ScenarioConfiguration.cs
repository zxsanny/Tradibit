using Microsoft.EntityFrameworkCore;
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
                v => JsonConvert.DeserializeObject<Dictionary<string, decimal?>>(v) ?? new Dictionary<string, decimal?>());
        
        builder.OwnsOne(sc => sc.Pair, p =>
        {
            p.OwnsOne(x => x.BaseCurrency);
            p.Navigation(x => x.BaseCurrency).IsRequired();
            
            p.OwnsOne(x => x.QuoteCurrency);
            p.Navigation(x => x.QuoteCurrency).IsRequired();
        });
        builder.Navigation(c => c.Pair).IsRequired();
        
        builder.HasOne(x => x.Strategy)
            .WithMany(x => x.Scenarios)
            .HasForeignKey(x => x.StrategyId);
        
        


    }
}