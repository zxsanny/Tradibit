using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradibit.Shared.Entities;

namespace Tradibit.DataAccess.Configuration;

public class TransitionConfiguration : IEntityTypeConfiguration<Transition>
{
    public void Configure(EntityTypeBuilder<Transition> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Step)
            .WithMany(x => x.Transitions)
            .HasForeignKey(x => x.StepId);
    }
}