using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class CustomerClassConfiguration : IEntityTypeConfiguration<CustomerClass>
{
    public void Configure(EntityTypeBuilder<CustomerClass> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
