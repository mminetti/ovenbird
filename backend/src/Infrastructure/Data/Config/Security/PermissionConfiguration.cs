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

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Module)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasData(
            new Permission
            {
                Id = 1,
                Name = Constants.Permissions.UsersRead,
                ModuleId = Constants.PermissionModules.Security,
                Description = "Allows viewing users."
            },
            new Permission
            {
                Id = 2,
                Name = Constants.Permissions.UsersWrite,
                ModuleId = Constants.PermissionModules.Security,
                Description = "Allows creating, updating, and deleting users."
            },
            new Permission
            {
                Id = 3,
                Name = Constants.Permissions.RolesRead,
                ModuleId = Constants.PermissionModules.Security,
                Description = "Allows viewing roles."
            },
            new Permission
            {
                Id = 4,
                Name = Constants.Permissions.RolesWrite,
                ModuleId = Constants.PermissionModules.Security,
                Description = "Allows creating, updating, and deleting roles."
            },
            new Permission
            {
                Id = 5,
                Name = Constants.Permissions.PermissionsRead,
                ModuleId = Constants.PermissionModules.Security,
                Description = "Allows viewing permissions."
            },
            new Permission
            {
                Id = 6,
                Name = Constants.Permissions.PermissionsWrite,
                ModuleId = Constants.PermissionModules.Security,
                Description = "Allows creating, updating, and deleting permissions."
            },
            new Permission
            {
                Id = 7,
                Name = Constants.Permissions.ConnectorsRead,
                ModuleId = Constants.PermissionModules.Settings,
                Description = "Allows viewing connectors."
            },
            new Permission
            {
                Id = 8,
                Name = Constants.Permissions.ConnectorsWrite,
                ModuleId = Constants.PermissionModules.Settings,
                Description = "Allows creating, updating, and deleting connectors."
            },
            new Permission
            {
                Id = 9,
                Name = Constants.Permissions.CompaniesRead,
                ModuleId = Constants.PermissionModules.Settings,
                Description = "Allows viewing companies."
            },
            new Permission
            {
                Id = 10,
                Name = Constants.Permissions.CompaniesWrite,
                ModuleId = Constants.PermissionModules.Settings,
                Description = "Allows creating, updating, and deleting companies."
            },
            new Permission
            {
                Id = 11,
                Name = Constants.Permissions.MarketsRead,
                ModuleId = Constants.PermissionModules.Settings,
                Description = "Allows viewing markets."
            },
            new Permission
            {
                Id = 12,
                Name = Constants.Permissions.MarketsWrite,
                ModuleId = Constants.PermissionModules.Settings,
                Description = "Allows creating, updating, and deleting markets."
            },
            new Permission
            {
                Id = 13,
                Name = Constants.Permissions.ConfigurationsRead,
                ModuleId = Constants.PermissionModules.Settings,
                Description = "Allows viewing configurations."
            },
            new Permission
            {
                Id = 14,
                Name = Constants.Permissions.ConfigurationsWrite,
                ModuleId = Constants.PermissionModules.Settings,
                Description = "Allows creating, updating, and deleting configurations."
            }
        );
    }
}
