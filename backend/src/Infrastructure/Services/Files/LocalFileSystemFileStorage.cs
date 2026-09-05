using UseCases.Interfaces.Files;

namespace Infrastructure.Services.Files;

/// <summary>
/// Stores uploaded documents on local disk instead of a real blob storage backend.
/// Not for production use - intended to let the BigData import strategy be exercised
/// end-to-end while running the app locally. "connection.string" is used as the base
/// directory on disk and "root.directory" as a subfolder under it, mirroring
/// AzureBlobFileStorage's connection-string/container split.
/// </summary>
public class LocalFileSystemFileStorage : IFileStorage
{
    public async Task<string> UploadAsync(FileStorageOptions options, Stream content, string remotePath, CancellationToken ct)
    {
        var path = ResolvePath(options, remotePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await content.CopyToAsync(fileStream, ct);

        return $"{options.RootDirectory}/{remotePath}";
    }

    public Task<Stream> OpenReadAsync(FileStorageOptions options, string remotePath, CancellationToken ct)
    {
        var path = ResolvePath(options, ExtractBlobName(remotePath));

        Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

        return Task.FromResult(stream);
    }

    private static string ResolvePath(FileStorageOptions options, string relativePath)
    {
        var rootDirectory = Path.GetFullPath(Path.Combine(options.ConnectionString, options.RootDirectory));
        var fullPath = Path.GetFullPath(Path.Combine(rootDirectory, relativePath));

        if (!fullPath.StartsWith(rootDirectory, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Path '{relativePath}' resolves outside of the configured root directory.");
        }

        return fullPath;
    }

    private static string ExtractBlobName(string fileReference)
    {
        var separatorIndex = fileReference.IndexOf('/');

        return separatorIndex >= 0 ? fileReference[(separatorIndex + 1)..] : fileReference;
    }
}
