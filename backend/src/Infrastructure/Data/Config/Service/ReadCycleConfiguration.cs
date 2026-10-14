using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class ReadCycleConfiguration : IEntityTypeConfiguration<ReadCycle>
{
    public void Configure(EntityTypeBuilder<ReadCycle> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}
