using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradibit.Shared.Entities;

namespace Tradibit.DataAccess.Configuration;

public class OperationsConfiguration : IEntityTypeConfiguration<BaseOperation>
{
    public void Configure(EntityTypeBuilder<BaseOperation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Transition)
            .WithMany(x => x.SuccessOperations)
            .HasForeignKey(x => x.TransitionId);
        
        builder.Ignore(x => x.Scenario);
        builder.Ignore(x => x.KlineUpdateEvent);

        builder.HasDiscriminator<int>("OperationType")
            .HasValue<OrderBaseOperation>(1)
            .HasValue<SetOperandBaseOperation>(2);
    }
}

public class SetOperandOperationConfiguration : IEntityTypeConfiguration<SetOperandBaseOperation>
{
    public void Configure(EntityTypeBuilder<SetOperandBaseOperation> builder)
    {
        builder.OwnsOne(x => x.OperandSource);
        builder.OwnsOne(x => x.OperandTo);
    }
}