using Core.Settings;

namespace UseCases.Interfaces.Files;

public interface IConnectorFtpTransport
{
    Task<IReadOnlyList<string>> ListFilesAsync(Configuration configuration, CancellationToken ct);
    Task<Stream> DownloadFileAsync(Configuration configuration, string remoteFilePath, CancellationToken ct);
    Task UploadFileAsync(Configuration configuration, Stream content, string remoteFileName, CancellationToken ct);
}
