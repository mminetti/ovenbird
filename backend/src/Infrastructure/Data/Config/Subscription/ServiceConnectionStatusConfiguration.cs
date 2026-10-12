using Core.Common.Constants;
using Core.Subscription;

namespace Infrastructure.Data.Config.Subscription;

public class ServiceConnectionStatusConfiguration : IEntityTypeConfiguration<ServiceConnectionStatus>
{
    public void Configure(EntityTypeBuilder<ServiceConnectionStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ServiceConnectionStatus { Id = Constants.ServiceConnectionStatuses.PendingDisconnection, Name = nameof(Constants.ServiceConnectionStatuses.PendingDisconnection) },
            new ServiceConnectionStatus { Id = Constants.ServiceConnectionStatuses.Disconnected, Name = nameof(Constants.ServiceConnectionStatuses.Disconnected) },
            new ServiceConnectionStatus { Id = Constants.ServiceConnectionStatuses.PendingConnection, Name = nameof(Constants.ServiceConnectionStatuses.PendingConnection) },
            new ServiceConnectionStatus { Id = Constants.ServiceConnectionStatuses.Connected, Name = nameof(Constants.ServiceConnectionStatuses.Connected) });
    }
}
