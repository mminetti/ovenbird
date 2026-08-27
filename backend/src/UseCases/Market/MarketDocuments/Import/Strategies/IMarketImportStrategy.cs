using Core.Shared;

namespace UseCases.Market.MarketDocuments.Import.Strategies;

public interface IMarketImportStrategy
{
    string MarketIdentifier { get; }
    Task<IReadOnlyList<string>> ListFilesAsync(Configuration configuration, CancellationToken ct);
    Task<Stream> DownloadFileAsync(Configuration configuration, string remoteFilePath, CancellationToken ct);
    Task UploadFileAsync(Configuration configuration, Stream content, string remoteFilePath, CancellationToken ct);
    Task<string> UploadDocumentAsync(Configuration configuration, Stream content, string remoteFilePath, CancellationToken ct);
}
