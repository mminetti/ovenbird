using Core.Common.Constants;
using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class ServiceStatusConfiguration : IEntityTypeConfiguration<ServiceStatus>
{
    public void Configure(EntityTypeBuilder<ServiceStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceStatus { Id = Constants.ServiceStatuses.New, Name = nameof(Constants.ServiceStatuses.New) },
            new ServiceStatus { Id = Constants.ServiceStatuses.InProgress, Name = nameof(Constants.ServiceStatuses.InProgress) },
            new ServiceStatus { Id = Constants.ServiceStatuses.Scheduled, Name = nameof(Constants.ServiceStatuses.Scheduled) },
            new ServiceStatus { Id = Constants.ServiceStatuses.Cancelled, Name = nameof(Constants.ServiceStatuses.Cancelled) },
            new ServiceStatus { Id = Constants.ServiceStatuses.Rejected, Name = nameof(Constants.ServiceStatuses.Rejected) },
            new ServiceStatus { Id = Constants.ServiceStatuses.Completed, Name = nameof(Constants.ServiceStatuses.Completed) },
            new ServiceStatus { Id = Constants.ServiceStatuses.Unexecuted, Name = nameof(Constants.ServiceStatuses.Unexecuted) },
            new ServiceStatus { Id = Constants.ServiceStatuses.Error, Name = nameof(Constants.ServiceStatuses.Error) });
    }
}
