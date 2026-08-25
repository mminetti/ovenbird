using Core.Shared;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments;

public class BigDataMarketDocumentStrategy(ISftpService sftpService) : IMarketDocumentStrategy
{
    public const string Identifier = "BigData";

    public string MarketIdentifier => Identifier;

    public Task<IReadOnlyList<string>> ListFilesAsync(Company company, string remoteDirectory, CancellationToken ct)
    {
        return sftpService.ListAsync(remoteDirectory, ct);
    }

    public Task<Stream> DownloadFileAsync(Company company, string remoteFilePath, CancellationToken ct)
    {
        return sftpService.DownloadAsync(remoteFilePath, ct);
    }

    public Task UploadFileAsync(string remoteFilePath, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
