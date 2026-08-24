using Core.Shared;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments;

public class BigDataMarketDocumentStrategy(IFtpService ftpService) : IMarketDocumentStrategy
{
    public const string Identifier = "default";

    public string MarketIdentifier => Identifier;

    public Task<IReadOnlyList<string>> ListFilesAsync(Company company, string remoteDirectory, CancellationToken ct)
    { 
        return ftpService.ListFilesAsync(remoteDirectory, ct);
    }

    public Task<Stream> DownloadAsync(Company company, string remoteFilePath, CancellationToken ct)
    {
        return ftpService.DownloadAsync(remoteFilePath, ct);
    }

    public Task UploadAsync(string remoteFilePath, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
