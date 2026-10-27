using Core.MarketExchange;
using Core.Settings;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public interface IEdiImportHandler
{
    string Identifier { get; }

    Task<EdiImportResult> ExecuteImportAsync(Configuration configuration, CancellationToken ct);

    Task<IReadOnlyList<MarketDocumentItem>> ExecuteItemImportAsync(Configuration configuration, MarketDocument document, CancellationToken ct);
}
