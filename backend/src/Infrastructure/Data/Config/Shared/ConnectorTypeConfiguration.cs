using Core.Constants;
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
                Id = 1,
                Name = ConnectorTypes.Ftp,
                Description = "FTP/SFTP file transfer connector."
            },
            new ConnectorType
            {
                Id = 2,
                Name = ConnectorTypes.FileStorage,
                Description = "File storage connector."
            }
        );
    }
}
