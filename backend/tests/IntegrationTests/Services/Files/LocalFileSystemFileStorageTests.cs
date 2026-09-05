using System.Text;
using Infrastructure.Services.Files;
using UseCases.Interfaces.Files;

namespace IntegrationTests.Services.Files;

public class LocalFileSystemFileStorageTests : IDisposable
{
    private readonly string _baseDirectory = Path.Combine(Path.GetTempPath(), "ovenbird-tests", Guid.NewGuid().ToString());
    private readonly LocalFileSystemFileStorage _storage = new();

    [Fact]
    public async Task UploadAsync_Then_OpenReadAsync_RoundTripsContentOnDisk()
    {
        var ct = TestContext.Current.CancellationToken;

        var options = CreateOptions();
        var expected = $"hello from local disk {Guid.NewGuid()}";
        var remotePath = $"{Guid.NewGuid()}.txt";

        await using (var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(expected)))
        {
            var reference = await _storage.UploadAsync(options, uploadStream, remotePath, ct);

            reference.ShouldBe($"{options.RootDirectory}/{remotePath}");

            await using var downloadStream = await _storage.OpenReadAsync(options, reference, ct);
            using var reader = new StreamReader(downloadStream);
            var actual = await reader.ReadToEndAsync(ct);

            actual.ShouldBe(expected);
        }
    }

    [Fact]
    public async Task UploadAsync_CreatesNestedDirectories_ForNestedRemotePaths()
    {
        var ct = TestContext.Current.CancellationToken;

        var options = CreateOptions();
        var remotePath = "edi/import/acme/2026/01/15/file.csv";

        await using (var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes("content")))
        {
            await _storage.UploadAsync(options, uploadStream, remotePath, ct);
        }

        var expectedPath = Path.Combine(_baseDirectory, options.RootDirectory, "edi", "import", "acme", "2026", "01", "15", "file.csv");
        File.Exists(expectedPath).ShouldBeTrue();
    }

    [Fact]
    public async Task OpenReadAsync_ThrowsWhenReferenceResolvesOutsideRootDirectory()
    {
        var ct = TestContext.Current.CancellationToken;

        var options = CreateOptions();

        await Should.ThrowAsync<InvalidOperationException>(
            () => _storage.OpenReadAsync(options, $"{options.RootDirectory}/../../escaped.txt", ct));
    }

    private FileStorageOptions CreateOptions() => new()
    {
        ConnectionString = _baseDirectory,
        RootDirectory = "import-market-documents",
    };

    public void Dispose()
    {
        if (Directory.Exists(_baseDirectory))
        {
            Directory.Delete(_baseDirectory, recursive: true);
        }
    }
}
