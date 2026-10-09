using Core.Common.Constants;
using Core.Service;

namespace Infrastructure.Data.Config.Service;

public class CommodityConfiguration : IEntityTypeConfiguration<Commodity>
{
    public void Configure(EntityTypeBuilder<Commodity> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new Commodity { Id = Constants.Commodities.Electricity, Name = nameof(Constants.Commodities.Electricity) });
    }
}
