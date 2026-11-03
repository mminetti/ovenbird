using Core.Common.Constants;
using Core.MarketExchange;

namespace Infrastructure.Data.Config.MarketExchange;

public class MarketDocumentEventStatusConfiguration : IEntityTypeConfiguration<MarketDocumentEventStatus>
{
    public void Configure(EntityTypeBuilder<MarketDocumentEventStatus> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new MarketDocumentEventStatus { Id = Constants.MarketDocumentEventStatuses.Done, Name = nameof(Constants.MarketDocumentEventStatuses.Done) },
            new MarketDocumentEventStatus { Id = Constants.MarketDocumentEventStatuses.Error, Name = nameof(Constants.MarketDocumentEventStatuses.Error) });
    }
}
