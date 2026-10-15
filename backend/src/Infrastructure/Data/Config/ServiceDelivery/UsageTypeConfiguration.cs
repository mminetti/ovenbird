using Core.Common.Constants;
using Core.ServiceDelivery;

namespace Infrastructure.Data.Config.ServiceDelivery;

public class UsageTypeConfiguration : IEntityTypeConfiguration<UsageType>
{
    public void Configure(EntityTypeBuilder<UsageType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new UsageType { Id = Constants.UsageTypes.Original, Name = nameof(Constants.UsageTypes.Original) },
            new UsageType { Id = Constants.UsageTypes.Cancel, Name = nameof(Constants.UsageTypes.Cancel) },
            new UsageType { Id = Constants.UsageTypes.Replace, Name = nameof(Constants.UsageTypes.Replace) });
    }
}
