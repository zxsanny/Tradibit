using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradibit.Shared.Entities;

namespace Tradibit.DataAccess.Configuration;

public class OperationsConfiguration : IEntityTypeConfiguration<OperationBase>
{
    public void Configure(EntityTypeBuilder<OperationBase> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Transition)
            .WithMany(x => x.SuccessOperations)
            .HasForeignKey(x => x.TransitionId);
        
        builder.Ignore(x => x.Scenario);
        builder.Ignore(x => x.KlineUpdateEvent);

        builder.HasDiscriminator<int>("OperationType")
            .HasValue<OrderOperation>(1)
            .HasValue<SetOperandOperation>(2);
    }
}