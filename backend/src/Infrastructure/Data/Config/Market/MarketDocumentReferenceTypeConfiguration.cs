using Core.Common.Constants;
using Core.Market;

namespace Infrastructure.Data.Config.Market;

public class MarketDocumentReferenceTypeConfiguration : IEntityTypeConfiguration<MarketDocumentReferenceType>
{
    public void Configure(EntityTypeBuilder<MarketDocumentReferenceType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new MarketDocumentReferenceType { Id = Constants.MarketDocumentReferenceTypes.HistoricalUsage, Name = nameof(Constants.MarketDocumentReferenceTypes.HistoricalUsage) },
            new MarketDocumentReferenceType { Id = Constants.MarketDocumentReferenceTypes.Usage, Name = nameof(Constants.MarketDocumentReferenceTypes.Usage) },
            new MarketDocumentReferenceType { Id = Constants.MarketDocumentReferenceTypes.Invoice, Name = nameof(Constants.MarketDocumentReferenceTypes.Invoice) },
            new MarketDocumentReferenceType { Id = Constants.MarketDocumentReferenceTypes.ServiceStart, Name = nameof(Constants.MarketDocumentReferenceTypes.ServiceStart) },
            new MarketDocumentReferenceType { Id = Constants.MarketDocumentReferenceTypes.ServiceEnd, Name = nameof(Constants.MarketDocumentReferenceTypes.ServiceEnd) },
            new MarketDocumentReferenceType { Id = Constants.MarketDocumentReferenceTypes.ServiceRequest, Name = nameof(Constants.MarketDocumentReferenceTypes.ServiceRequest) });
    }
}
