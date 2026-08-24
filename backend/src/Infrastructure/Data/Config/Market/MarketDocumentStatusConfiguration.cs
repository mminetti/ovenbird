using Core.Constants;
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
            new MarketDocumentStatus { Id = MarketDocumentStatuses.New, Name = nameof(MarketDocumentStatuses.New) },
            new MarketDocumentStatus { Id = MarketDocumentStatuses.Done, Name = nameof(MarketDocumentStatuses.Done) },
            new MarketDocumentStatus { Id = MarketDocumentStatuses.Error, Name = nameof(MarketDocumentStatuses.Error) });
    }
}
