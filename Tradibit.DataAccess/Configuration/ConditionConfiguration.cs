using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradibit.Shared.Entities;

namespace Tradibit.DataAccess.Configuration;

public class ConditionConfiguration : IEntityTypeConfiguration<Condition>
{
    public void Configure(EntityTypeBuilder<Condition> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Operand1);
        builder.OwnsOne(x => x.Operand2);

        builder.HasOne(x => x.Transition)
            .WithMany(x => x.Conditions)
            .HasForeignKey(x => x.TransitionId);
    }
}