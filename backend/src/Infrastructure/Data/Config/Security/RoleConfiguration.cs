using Core.Security;

namespace Infrastructure.Data.Config.Security;

public class RoleConfiguration : BaseEntityTypeConfiguration<Role, int>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // Cascade: join table row only, never deletes either side.
        builder.HasMany(x => x.Permissions)
            .WithMany(x => x.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "RolePermission",
                j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                j =>
                {
                    j.ToTable("RolePermission");
                    j.HasKey("PermissionId", "RoleId");
                });
    }
}
