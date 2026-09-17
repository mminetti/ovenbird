using Core.Security;
using UseCases.Common;

namespace Infrastructure.Data.Config.Security;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.HasData(
            new Permission
            {
                Id = 1,
                Name = Constants.Permissions.UsersRead,
                Description = "Allows viewing users."
            },
            new Permission
            {
                Id = 2,
                Name = Constants.Permissions.UsersWrite,
                Description = "Allows creating, updating, and deleting users."
            },
            new Permission
            {
                Id = 3,
                Name = Constants.Permissions.RolesRead,
                Description = "Allows viewing roles."            
            },
            new Permission
            {
                Id = 4,
                Name = Constants.Permissions.RolesWrite,
                Description = "Allows creating, updating, and deleting roles."
            },
            new Permission
            {
                Id = 5,
                Name = Constants.Permissions.PermissionsRead,
                Description = "Allows viewing permissions."
            },
            new Permission
            {
                Id = 6,
                Name = Constants.Permissions.PermissionsWrite,
                Description = "Allows creating, updating, and deleting permissions."
            },
            new Permission
            {
                Id = 7,
                Name = Constants.Permissions.ConnectorsRead,
                Description = "Allows viewing connectors."
            },
            new Permission
            {
                Id = 8,
                Name = Constants.Permissions.ConnectorsWrite,
                Description = "Allows creating, updating, and deleting connectors."
            }
        );
    }
}
