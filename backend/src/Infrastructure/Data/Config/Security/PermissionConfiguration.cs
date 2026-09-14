using Core.Security;
using UseCases.Common;

namespace Infrastructure.Data.Config.Security;

public class PermissionConfiguration : BaseEntityTypeConfiguration<Permission, int>
{
    // Negative, hardcoded IDs reserved for seed data - real Permission rows always get a
    // positive value from the IDENTITY column, so seeded and organically-created rows can
    // never collide, no matter what already exists in the table.
    private static readonly DateTimeOffset SeedDate = new(2026, 9, 14, 0, 0, 0, TimeSpan.Zero);

    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.HasData(
            new Permission
            {
                Id = -1,
                Name = Constants.Permissions.UsersRead,
                Description = "Allows viewing users.",
                CreatedAtUtc = SeedDate,
                LastModifiedAtUtc = SeedDate
            },
            new Permission
            {
                Id = -2,
                Name = Constants.Permissions.UsersWrite,
                Description = "Allows creating, updating, and deleting users.",
                CreatedAtUtc = SeedDate,
                LastModifiedAtUtc = SeedDate
            },
            new Permission
            {
                Id = -3,
                Name = Constants.Permissions.RolesRead,
                Description = "Allows viewing roles.",
                CreatedAtUtc = SeedDate,
                LastModifiedAtUtc = SeedDate
            },
            new Permission
            {
                Id = -4,
                Name = Constants.Permissions.RolesWrite,
                Description = "Allows creating, updating, and deleting roles.",
                CreatedAtUtc = SeedDate,
                LastModifiedAtUtc = SeedDate
            },
            new Permission
            {
                Id = -5,
                Name = Constants.Permissions.PermissionsRead,
                Description = "Allows viewing permissions.",
                CreatedAtUtc = SeedDate,
                LastModifiedAtUtc = SeedDate
            },
            new Permission
            {
                Id = -6,
                Name = Constants.Permissions.PermissionsWrite,
                Description = "Allows creating, updating, and deleting permissions.",
                CreatedAtUtc = SeedDate,
                LastModifiedAtUtc = SeedDate
            }
        );
    }
}
