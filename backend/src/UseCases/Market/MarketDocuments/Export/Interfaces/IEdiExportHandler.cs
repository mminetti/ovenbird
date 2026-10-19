using Core.Settings;

namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public interface IEdiExportHandler
{
    string Identifier { get; }

    Task<MarketDocumentExportResult?> ExecuteExportAsync(Configuration configuration, CancellationToken ct);
}
