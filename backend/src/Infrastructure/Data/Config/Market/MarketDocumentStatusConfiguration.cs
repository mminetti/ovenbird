using Core.Common;
using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentStatusConfiguration : IEntityTypeConfiguration<MarketDocumentStatus>
{
    public void Configure(EntityTypeBuilder<MarketDocumentStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new MarketDocumentStatus { Id = Constants.MarketDocumentStatuses.New, Name = nameof(Constants.MarketDocumentStatuses.New) },
            new MarketDocumentStatus { Id = Constants.MarketDocumentStatuses.Done, Name = nameof(Constants.MarketDocumentStatuses.Done) },
            new MarketDocumentStatus { Id = Constants.MarketDocumentStatuses.Error, Name = nameof(Constants.MarketDocumentStatuses.Error) });
    }
}
