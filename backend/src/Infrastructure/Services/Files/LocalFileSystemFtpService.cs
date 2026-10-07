using UseCases.Interfaces.Files;

namespace Infrastructure.Services.Files;

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

    private static string ResolveDirectory(FtpOptions options) => Path.GetFullPath(options.RootDirectory);

    private static string ResolvePath(FtpOptions options, string remotePath)
    {
        var directory = ResolveDirectory(options);
        var fullPath = Path.GetFullPath(remotePath);

        if (!fullPath.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Remote path '{remotePath}' is outside of the configured root directory '{options.RootDirectory}'.");
        }

        return fullPath;
    }
}
