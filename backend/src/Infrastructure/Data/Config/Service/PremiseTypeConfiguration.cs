using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class PremiseTypeConfiguration : IEntityTypeConfiguration<PremiseType>
{
    public void Configure(EntityTypeBuilder<PremiseType> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
