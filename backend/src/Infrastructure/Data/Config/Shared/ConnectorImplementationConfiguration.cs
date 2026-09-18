using Core.Common;
using Core.Shared;
using Infrastructure.Services.Files;

namespace Infrastructure.Data.Config.Shared;

public class ConnectorImplementationConfiguration : IEntityTypeConfiguration<ConnectorImplementation>
{
    public void Configure(EntityTypeBuilder<ConnectorImplementation> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.ConnectorType)
            .WithMany(x => x.ConnectorImplementations)
            .HasForeignKey(x => x.ConnectorTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasData(
            new ConnectorImplementation
            {
                Id = Constants.ConnectorImplementations.FluentFtpService,
                Identifier = nameof(FluentFtpService),
                Name = "Fluent FTP",
                Description = "FTP connector implementation backed by FluentFTP.",
                ConnectorTypeId = Constants.ConnectorTypes.Ftp
            },
            new ConnectorImplementation
            {
                Id = Constants.ConnectorImplementations.SshNetSftpService,
                Identifier = nameof(SshNetSftpService),
                Name = "SSH.NET SFTP",
                Description = "SFTP connector implementation backed by SSH.NET.",
                ConnectorTypeId = Constants.ConnectorTypes.Ftp
            },
            new ConnectorImplementation
            {
                Id = Constants.ConnectorImplementations.LocalFileSystemFtpService,
                Identifier = nameof(LocalFileSystemFtpService),
                Name = "Local File System FTP",
                Description = "Local drop-folder stand-in for FTP/SFTP, for development use only.",
                ConnectorTypeId = Constants.ConnectorTypes.Ftp
            },
            new ConnectorImplementation
            {
                Id = Constants.ConnectorImplementations.AzureBlobFileStorage,
                Identifier = nameof(AzureBlobFileStorage),
                Name = "Azure Blob Storage",
                Description = "File storage connector implementation backed by Azure Blob Storage.",
                ConnectorTypeId = Constants.ConnectorTypes.FileStorage
            },
            new ConnectorImplementation
            {
                Id = Constants.ConnectorImplementations.LocalFileSystemFileStorage,
                Identifier = nameof(LocalFileSystemFileStorage),
                Name = "Local File System Storage",
                Description = "Local file system stand-in for file storage, for development use only.",
                ConnectorTypeId = Constants.ConnectorTypes.FileStorage
            }
        );
    }
}
