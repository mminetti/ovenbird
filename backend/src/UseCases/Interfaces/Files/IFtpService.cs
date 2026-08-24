namespace UseCases.Interfaces.Files;

public interface IFtpService
{
    Task<IReadOnlyList<string>> ListFilesAsync(string remoteDirectory, CancellationToken ct);
    Task UploadAsync(Stream content, string remotePath, CancellationToken ct);
    Task<Stream> DownloadAsync(string remotePath, CancellationToken ct);
}
