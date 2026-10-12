using Core.Subscription;

namespace Infrastructure.Data.Config.Subscription;

public class CustomerConfiguration : BaseEntityTypeConfiguration<Customer, long>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Identifier)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.FirstName)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(x => x.LastName)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
