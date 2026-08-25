namespace UseCases.Interfaces.Files;

public interface ISftpService
{
    Task<IReadOnlyList<string>> ListAsync(string remoteDirectory, CancellationToken ct);
    Task UploadAsync(Stream content, string remotePath, CancellationToken ct);
    Task<Stream> DownloadAsync(string remotePath, CancellationToken ct);
}
