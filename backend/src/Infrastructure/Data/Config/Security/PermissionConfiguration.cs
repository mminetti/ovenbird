using Core.Security;

namespace Infrastructure.Data.Config.Security;

public class PermissionConfiguration : BaseEntityTypeConfiguration<Permission, int>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        // NoAction: protect the referenced row from accidental/cascading deletion.
        builder.HasOne(x => x.Module)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
