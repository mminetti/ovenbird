using Core.Market;
using Core.Settings;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public interface IMarketDocumentImport
{
    string Identifier { get; }
    Task<IReadOnlyList<string>> ListFilesAsync(Configuration configuration, CancellationToken ct);
    Task<Stream> DownloadFileAsync(Configuration configuration, string remoteFilePath, CancellationToken ct);
    Task<string> UploadDocumentAsync(Configuration configuration, Stream content, string fileName, CancellationToken ct);
    Task<Stream> OpenDocumentAsync(Configuration configuration, string fileReference, CancellationToken ct);
    Task<IReadOnlyList<MarketDocumentItem>> ReadTransactionsAsync(Stream content, CancellationToken ct);
}
