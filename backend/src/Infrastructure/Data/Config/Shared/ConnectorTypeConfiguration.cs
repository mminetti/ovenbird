using Core.Common;
using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class ConnectorTypeConfiguration : IEntityTypeConfiguration<ConnectorType>
{
    public void Configure(EntityTypeBuilder<ConnectorType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.HasData(
            new ConnectorType
            {
                Id = Constants.ConnectorTypes.Ftp,
                Name = "FTP",
                Description = "FTP/SFTP file transfer connector."
            },
            new ConnectorType
            {
                Id = Constants.ConnectorTypes.FileStorage,
                Name = "File Storage",
                Description = "File storage connector."
            }
        );
    }
}
