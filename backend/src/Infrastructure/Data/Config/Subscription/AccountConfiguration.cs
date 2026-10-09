using Core.Subscription;

namespace Infrastructure.Data.Config.Subscription;

public class AccountConfiguration : BaseEntityTypeConfiguration<Account, long>
{
    public override void Configure(EntityTypeBuilder<Account> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
