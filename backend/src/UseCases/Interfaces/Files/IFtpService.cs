namespace UseCases.Interfaces.Files;

public interface IFtpService
{
    Task UploadAsync(Stream content, string remotePath, CancellationToken ct);
    Task<Stream> DownloadAsync(string remotePath, CancellationToken ct);
}
