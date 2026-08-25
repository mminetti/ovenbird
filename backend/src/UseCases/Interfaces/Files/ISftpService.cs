namespace UseCases.Interfaces.Files;

public interface ISftpService
{
    Task<IReadOnlyList<string>> ListAsync(SftpOptions options, CancellationToken ct);
    Task UploadAsync(SftpOptions options, Stream content, string remotePath, CancellationToken ct);
    Task<Stream> DownloadAsync(SftpOptions options, string remotePath, CancellationToken ct);
}
