using Core.Security;

namespace Infrastructure.Data.Config.Security;

public class UserConfiguration : BaseEntityTypeConfiguration<User, int>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.ExternalIdentifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.HasIndex(x => x.ExternalIdentifier)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Email)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        // Cascade: join table row only, never deletes either side.
        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserRole",
                j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                j =>
                {
                    j.ToTable("UserRole");
                    j.HasKey("RoleId", "UserId");
                });
    }
}
