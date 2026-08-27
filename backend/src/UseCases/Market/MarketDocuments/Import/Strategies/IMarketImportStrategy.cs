using Core.Shared;

namespace UseCases.Market.MarketDocuments.Import.Strategies;

public interface IMarketImportStrategy
{
    string MarketIdentifier { get; }
    Task<IReadOnlyList<string>> ListFilesAsync(Integration integration, CancellationToken ct);
    Task<Stream> DownloadFileAsync(Integration integration, string remoteFilePath, CancellationToken ct);
    Task UploadFileAsync(Integration integration, Stream content, string remoteFilePath, CancellationToken ct);
    Task<string> UploadDocumentAsync(Integration integration, Stream content, string remoteFilePath, CancellationToken ct);
}
