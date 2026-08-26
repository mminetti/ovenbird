namespace UseCases.Interfaces.Files;

public interface IFtpService
{
    Task<IReadOnlyList<string>> ListAsync(FtpOptions options, CancellationToken ct);
    Task UploadAsync(FtpOptions options, Stream content, string remotePath, CancellationToken ct);
    Task<Stream> DownloadAsync(FtpOptions options, string remotePath, CancellationToken ct);
}
