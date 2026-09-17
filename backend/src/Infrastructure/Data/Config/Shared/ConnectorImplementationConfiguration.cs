using Core.Constants;
using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class ConnectorImplementationConfiguration : IEntityTypeConfiguration<ConnectorImplementation>
{
    public void Configure(EntityTypeBuilder<ConnectorImplementation> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.HasData(
            new ConnectorImplementation
            {
                Id = 1,
                Name = ConnectorImplementations.FluentFtpService,
                Description = "FTP connector implementation backed by FluentFTP."
            },
            new ConnectorImplementation
            {
                Id = 2,
                Name = ConnectorImplementations.SshNetSftpService,
                Description = "SFTP connector implementation backed by SSH.NET."
            },
            new ConnectorImplementation
            {
                Id = 3,
                Name = ConnectorImplementations.LocalFileSystemFtpService,
                Description = "Local drop-folder stand-in for FTP/SFTP, for development use only."
            },
            new ConnectorImplementation
            {
                Id = 4,
                Name = ConnectorImplementations.AzureBlobFileStorage,
                Description = "File storage connector implementation backed by Azure Blob Storage."
            },
            new ConnectorImplementation
            {
                Id = 5,
                Name = ConnectorImplementations.LocalFileSystemFileStorage,
                Description = "Local file system stand-in for file storage, for development use only."
            }
        );
    }
}
