using UseCases.Interfaces.Files;

namespace UnitTests.UseCases.Market.MarketDocuments.TestDoubles;

/// <summary>
/// In-memory stand-in for the real blob/file storage backend, so the BigData import strategy and
/// ImportMarketDocumentHandler can be exercised end-to-end without a storage emulator dependency.
/// Mirrors AzureBlobFileStorage's reference format ("{RootDirectory}/{remotePath}") so tests can
/// assert on the same shape of storage key that production code returns.
/// </summary>
public class TestFileStorage : IFileStorage
{
    private readonly Dictionary<string, byte[]> _blobs = new(StringComparer.OrdinalIgnoreCase);

    public List<string> UploadedPaths { get; } = [];

    public async Task<string> UploadAsync(FileStorageOptions options, Stream content, string remotePath, CancellationToken ct)
    {
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, ct);
        _blobs[remotePath] = buffer.ToArray();
        UploadedPaths.Add(remotePath);

        return $"{options.RootDirectory}/{remotePath}";
    }

    public Task<Stream> OpenReadAsync(FileStorageOptions options, string remotePath, CancellationToken ct)
    {
        var blobName = ExtractBlobName(remotePath);

        if (!_blobs.TryGetValue(blobName, out var content))
        {
            throw new FileNotFoundException($"Blob '{blobName}' was not found in the test file storage.");
        }

        return Task.FromResult<Stream>(new MemoryStream(content));
    }

    private static string ExtractBlobName(string fileReference)
    {
        var separatorIndex = fileReference.IndexOf('/');

        return separatorIndex >= 0 ? fileReference[(separatorIndex + 1)..] : fileReference;
    }
}
