using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Tradibit.Shared.Entities;

namespace Tradibit.DataAccess.Configuration;

public class UserStateConfiguration : IEntityTypeConfiguration<UserState>
{
    public void Configure(EntityTypeBuilder<UserState> builder)
    {
        builder.HasKey(us => us.Id);
        
        builder.Property(us => us.ActivePairs)
            .HasConversion(v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<ActivePair>>(v) ?? new List<ActivePair>())
            .Metadata.SetValueComparer(new ValueComparer<List<ActivePair>>(
                (p1, p2) => p1!.SequenceEqual(p2!),
                list => list.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                list => list.ToList()));
    }
}
