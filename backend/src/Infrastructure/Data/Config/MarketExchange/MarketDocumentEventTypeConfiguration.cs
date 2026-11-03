using Core.Common.Constants;
using Core.MarketExchange;

namespace Infrastructure.Data.Config.MarketExchange;

public class MarketDocumentEventTypeConfiguration : IEntityTypeConfiguration<MarketDocumentEventType>
{
    public void Configure(EntityTypeBuilder<MarketDocumentEventType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new MarketDocumentEventType { Id = Constants.MarketDocumentEventTypes.HistoricalUsage, Name = nameof(Constants.MarketDocumentEventTypes.HistoricalUsage) },
            new MarketDocumentEventType { Id = Constants.MarketDocumentEventTypes.Usage, Name = nameof(Constants.MarketDocumentEventTypes.Usage) },
            new MarketDocumentEventType { Id = Constants.MarketDocumentEventTypes.Invoice, Name = nameof(Constants.MarketDocumentEventTypes.Invoice) },
            new MarketDocumentEventType { Id = Constants.MarketDocumentEventTypes.ServiceStart, Name = nameof(Constants.MarketDocumentEventTypes.ServiceStart) },
            new MarketDocumentEventType { Id = Constants.MarketDocumentEventTypes.ServiceEnd, Name = nameof(Constants.MarketDocumentEventTypes.ServiceEnd) },
            new MarketDocumentEventType { Id = Constants.MarketDocumentEventTypes.ServiceRequest, Name = nameof(Constants.MarketDocumentEventTypes.ServiceRequest) });
    }
}
