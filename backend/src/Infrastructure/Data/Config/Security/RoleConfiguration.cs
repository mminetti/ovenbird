using Core.Security;

namespace Infrastructure.Data.Config.Security;

public class RoleConfiguration : BaseEntityTypeConfiguration<Role, int>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasMany(x => x.Permissions)
            .WithMany(x => x.Roles)
            .UsingEntity(x => x.ToTable("RolePermission"));
    }
}
