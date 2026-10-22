using Core.Common.Constants;
using Core.Processing;

namespace Infrastructure.Data.Config.Processing;

public class ProcessTypeConfiguration : IEntityTypeConfiguration<ProcessType>
{
    public void Configure(EntityTypeBuilder<ProcessType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.HasData(
            new ProcessType { Id = Constants.ProcessTypes.MarketDocumentImport, Name = nameof(Constants.ProcessTypes.MarketDocumentImport) },
            new ProcessType { Id = Constants.ProcessTypes.MarketDocumentExport, Name = nameof(Constants.ProcessTypes.MarketDocumentExport) },
            new ProcessType { Id = Constants.ProcessTypes.MarketDocumentItemExport, Name = nameof(Constants.ProcessTypes.MarketDocumentItemExport) });
    }
}
