namespace UseCases.Interfaces.Files;

public interface ISftpService
{
    Task<IReadOnlyList<string>> ListAsync(FtpOptions options, CancellationToken ct);
    Task UploadAsync(FtpOptions options, Stream content, string remotePath, CancellationToken ct);
    Task<Stream> DownloadAsync(FtpOptions options, string remotePath, CancellationToken ct);
}
