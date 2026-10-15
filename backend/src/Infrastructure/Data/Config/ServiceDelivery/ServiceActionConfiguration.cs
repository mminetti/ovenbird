using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceActionConfiguration : IEntityTypeConfiguration<ServiceAction>
{
    public void Configure(EntityTypeBuilder<ServiceAction> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceAction { Id = Constants.ServiceActions.Initiate, Name = nameof(Constants.ServiceActions.Initiate) },
            new ServiceAction { Id = Constants.ServiceActions.Confirm, Name = nameof(Constants.ServiceActions.Confirm) },
            new ServiceAction { Id = Constants.ServiceActions.Reject, Name = nameof(Constants.ServiceActions.Reject) },
            new ServiceAction { Id = Constants.ServiceActions.Complete, Name = nameof(Constants.ServiceActions.Complete) },
            new ServiceAction { Id = Constants.ServiceActions.Cancel, Name = nameof(Constants.ServiceActions.Cancel) },
            new ServiceAction { Id = Constants.ServiceActions.Unexecute, Name = nameof(Constants.ServiceActions.Unexecute) },
            new ServiceAction { Id = Constants.ServiceActions.Error, Name = nameof(Constants.ServiceActions.Error) },
            new ServiceAction { Id = Constants.ServiceActions.Change, Name = nameof(Constants.ServiceActions.Change) });
    }
}
