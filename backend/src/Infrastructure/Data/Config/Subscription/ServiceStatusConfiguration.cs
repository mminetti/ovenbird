using Core.Common.Constants;
using Core.Subscription;

namespace Infrastructure.Data.Config.Subscription;

public class ServiceStatusConfiguration : IEntityTypeConfiguration<ServiceStatus>
{
    public void Configure(EntityTypeBuilder<ServiceStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceStatus { Id = Constants.ServiceStatuses.Pending, Name = nameof(Constants.ServiceStatuses.Pending) },
            new ServiceStatus { Id = Constants.ServiceStatuses.On, Name = nameof(Constants.ServiceStatuses.On) },
            new ServiceStatus { Id = Constants.ServiceStatuses.Off, Name = nameof(Constants.ServiceStatuses.Off) });
    }
}
