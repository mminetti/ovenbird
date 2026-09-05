using UseCases.Interfaces.Files;

namespace Infrastructure.Services.Files;

/// <summary>
/// Reads/writes files from a local "drop folder" instead of a real FTP/SFTP server.
/// Not for production use - intended to let the BigData import strategy be exercised
/// end-to-end while running the app locally, by pointing a connector's
/// "ftp.remote.directory" field at a folder on disk. Host/username/password connector
/// fields are still required by BigDataImportStrategy but are ignored here.
/// </summary>
public class LocalFileSystemFtpService : IFtpService
{
    public Task<IReadOnlyList<string>> ListAsync(FtpOptions options, CancellationToken ct)
    {
        var directory = ResolveDirectory(options);

        if (!Directory.Exists(directory))
        {
            return Task.FromResult<IReadOnlyList<string>>([]);
        }

        IReadOnlyList<string> files = Directory
            .EnumerateFiles(directory)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Task.FromResult(files);
    }

    public async Task<Stream> DownloadAsync(FtpOptions options, string remotePath, CancellationToken ct)
    {
        var path = ResolvePath(options, remotePath);

        var buffer = new MemoryStream();
        await using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
        {
            await fileStream.CopyToAsync(buffer, ct);
        }
        buffer.Position = 0;

        return buffer;
    }

    public async Task UploadAsync(FtpOptions options, Stream content, string remotePath, CancellationToken ct)
    {
        var directory = ResolveDirectory(options);
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, Path.GetFileName(remotePath));

        await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await content.CopyToAsync(fileStream, ct);
    }

    private static string ResolveDirectory(FtpOptions options) => Path.GetFullPath(options.RemoteDirectory);

    private static string ResolvePath(FtpOptions options, string remotePath)
    {
        var directory = ResolveDirectory(options);
        var fullPath = Path.GetFullPath(remotePath);

        if (!fullPath.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Remote path '{remotePath}' is outside of the configured remote directory '{options.RemoteDirectory}'.");
        }

        return fullPath;
    }
}
