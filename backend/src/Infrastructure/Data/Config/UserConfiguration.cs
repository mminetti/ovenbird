using Core.Security;

namespace Infrastructure.Data.Config;

public class UserConfiguration : BaseEntityTypeConfiguration<User, int>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.ExternalIdentifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.Email)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity(x => x.ToTable("UserRole"));
    }
}
