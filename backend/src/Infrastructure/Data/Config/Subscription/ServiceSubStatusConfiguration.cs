using Core.Common.Constants;
using Core.Subscription;

namespace Infrastructure.Data.Config.Subscription;

public class ServiceSubStatusConfiguration : IEntityTypeConfiguration<ServiceSubStatus>
{
    public void Configure(EntityTypeBuilder<ServiceSubStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceSubStatus { Id = Constants.ServiceSubStatuses.PendingStart, Name = nameof(Constants.ServiceSubStatuses.PendingStart) },
            new ServiceSubStatus { Id = Constants.ServiceSubStatuses.Active, Name = nameof(Constants.ServiceSubStatuses.Active) },
            new ServiceSubStatus { Id = Constants.ServiceSubStatuses.PendingEnd, Name = nameof(Constants.ServiceSubStatuses.PendingEnd) },
            new ServiceSubStatus { Id = Constants.ServiceSubStatuses.Ended, Name = nameof(Constants.ServiceSubStatuses.Ended) },
            new ServiceSubStatus { Id = Constants.ServiceSubStatuses.Closed, Name = nameof(Constants.ServiceSubStatuses.Closed) });
    }
}
