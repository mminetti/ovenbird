using UseCases.Interfaces.Files;

namespace UnitTests.UseCases.Market.MarketDocuments.TestDoubles;

/// <summary>
/// In-memory stand-in for a real FTP/SFTP server, so the BigData import strategy and
/// ImportMarketDocumentHandler can be exercised end-to-end without a network dependency.
/// </summary>
public class TestFtpService : IFtpService
{
    private readonly Dictionary<string, byte[]> _files = new(StringComparer.OrdinalIgnoreCase);

    public List<string> DownloadedPaths { get; } = [];

    public TestFtpService SeedFile(string remotePath, string content)
    {
        _files[remotePath] = System.Text.Encoding.UTF8.GetBytes(content);
        return this;
    }

    public Task<IReadOnlyList<string>> ListAsync(FtpOptions options, CancellationToken ct)
    {
        IReadOnlyList<string> matches = _files.Keys
            .Where(path => path.StartsWith(options.RemoteDirectory, StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Task.FromResult(matches);
    }

    public Task<Stream> DownloadAsync(FtpOptions options, string remotePath, CancellationToken ct)
    {
        if (!_files.TryGetValue(remotePath, out var content))
        {
            throw new FileNotFoundException($"Remote file '{remotePath}' was not found on the test FTP server.");
        }

        DownloadedPaths.Add(remotePath);

        return Task.FromResult<Stream>(new MemoryStream(content));
    }

    public async Task UploadAsync(FtpOptions options, Stream content, string remotePath, CancellationToken ct)
    {
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, ct);
        _files[remotePath] = buffer.ToArray();
    }
}
