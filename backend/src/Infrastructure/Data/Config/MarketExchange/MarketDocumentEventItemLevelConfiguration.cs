using Core.Common.Constants;
using Core.MarketExchange;

namespace Infrastructure.Data.Config.MarketExchange;

public class MarketDocumentEventItemLevelConfiguration : IEntityTypeConfiguration<MarketDocumentEventItemLevel>
{
    public void Configure(EntityTypeBuilder<MarketDocumentEventItemLevel> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new MarketDocumentEventItemLevel { Id = Constants.MarketDocumentEventItemLevels.Error, Name = nameof(Constants.MarketDocumentEventItemLevels.Error) },
            new MarketDocumentEventItemLevel { Id = Constants.MarketDocumentEventItemLevels.Warning, Name = nameof(Constants.MarketDocumentEventItemLevels.Warning) },
            new MarketDocumentEventItemLevel { Id = Constants.MarketDocumentEventItemLevels.Info, Name = nameof(Constants.MarketDocumentEventItemLevels.Info) });
    }
}
