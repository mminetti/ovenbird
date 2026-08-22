using Core.Shared;

namespace Infrastructure.Data.Config.Shared;

public class CompanyConfiguration : BaseEntityTypeConfiguration<Company, int>
{
    public override void Configure(EntityTypeBuilder<Company> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
