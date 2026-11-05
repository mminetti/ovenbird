using Core.Common;
using Core.Common.Constants;

namespace Infrastructure.Data.Config.Common;

public class CaseEntityTypeConfiguration : IEntityTypeConfiguration<CaseEntityType>
{
    public void Configure(EntityTypeBuilder<CaseEntityType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new CaseEntityType { Id = Constants.CaseEntityTypes.ProcessRun, Name = nameof(Constants.CaseEntityTypes.ProcessRun) },
            new CaseEntityType { Id = Constants.CaseEntityTypes.MarketDocumentItem, Name = nameof(Constants.CaseEntityTypes.MarketDocumentItem) },
            new CaseEntityType { Id = Constants.CaseEntityTypes.MarketDocument, Name = nameof(Constants.CaseEntityTypes.MarketDocument) });
    }
}
