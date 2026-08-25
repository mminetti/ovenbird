using Core.Shared;

namespace UseCases.Market.MarketDocuments.Import.Strategies;

public interface IMarketImportStrategy
{
    string MarketIdentifier { get; }
    Task<IReadOnlyList<string>> ListFilesAsync(SystemIntegration integration, CancellationToken ct);
    Task<Stream> DownloadFileAsync(SystemIntegration integration, string remoteFilePath, CancellationToken ct);
    Task UploadFileAsync(SystemIntegration integration, Stream content, string remoteFilePath, CancellationToken ct);
    Task<string> UploadDocumentAsync(SystemIntegration integration, Stream content, string remoteFilePath, CancellationToken ct);
}
