using Core.Common.Constants;
using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentItemStatusConfiguration : IEntityTypeConfiguration<MarketDocumentItemStatus>
{
    public void Configure(EntityTypeBuilder<MarketDocumentItemStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new MarketDocumentItemStatus { Id = Constants.MarketDocumentItemStatuses.New, Name = nameof(Constants.MarketDocumentItemStatuses.New) },
            new MarketDocumentItemStatus { Id = Constants.MarketDocumentItemStatuses.Done, Name = nameof(Constants.MarketDocumentItemStatuses.Done) },
            new MarketDocumentItemStatus { Id = Constants.MarketDocumentItemStatuses.Error, Name = nameof(Constants.MarketDocumentItemStatuses.Error) });
    }
}
