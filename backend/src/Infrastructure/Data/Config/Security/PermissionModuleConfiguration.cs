using Core.Security;
using UseCases.Common;

namespace Infrastructure.Data.Config.Security;

public class PermissionModuleConfiguration : IEntityTypeConfiguration<PermissionModule>
{
    public void Configure(EntityTypeBuilder<PermissionModule> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new PermissionModule { Id = Constants.PermissionModules.Security, Name = "Security" },
            new PermissionModule { Id = Constants.PermissionModules.Settings, Name = "Settings" }
        );
    }
}
