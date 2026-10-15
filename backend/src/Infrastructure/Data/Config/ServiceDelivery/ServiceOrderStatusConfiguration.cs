using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class ServiceOrderStatusConfiguration : IEntityTypeConfiguration<ServiceOrderStatus>
{
    public void Configure(EntityTypeBuilder<ServiceOrderStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceOrderStatus { Id = Constants.ServiceOrderStatuses.New, Name = nameof(Constants.ServiceOrderStatuses.New) },
            new ServiceOrderStatus { Id = Constants.ServiceOrderStatuses.InProgress, Name = nameof(Constants.ServiceOrderStatuses.InProgress) },
            new ServiceOrderStatus { Id = Constants.ServiceOrderStatuses.Scheduled, Name = nameof(Constants.ServiceOrderStatuses.Scheduled) },
            new ServiceOrderStatus { Id = Constants.ServiceOrderStatuses.Cancelled, Name = nameof(Constants.ServiceOrderStatuses.Cancelled) },
            new ServiceOrderStatus { Id = Constants.ServiceOrderStatuses.Rejected, Name = nameof(Constants.ServiceOrderStatuses.Rejected) },
            new ServiceOrderStatus { Id = Constants.ServiceOrderStatuses.Completed, Name = nameof(Constants.ServiceOrderStatuses.Completed) },
            new ServiceOrderStatus { Id = Constants.ServiceOrderStatuses.Unexecuted, Name = nameof(Constants.ServiceOrderStatuses.Unexecuted) },
            new ServiceOrderStatus { Id = Constants.ServiceOrderStatuses.Error, Name = nameof(Constants.ServiceOrderStatuses.Error) });
    }
}
