using Core.Settings;

namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public interface IEdiExportHandler
{
    string Identifier { get; }

    Task<string> StoreTransactionAsync(Configuration configuration, Stream content, string fileName, CancellationToken ct);
}
