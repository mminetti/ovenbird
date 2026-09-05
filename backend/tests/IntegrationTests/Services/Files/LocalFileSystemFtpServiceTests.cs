using System.Text;
using Infrastructure.Services.Files;
using UseCases.Interfaces.Files;

namespace IntegrationTests.Services.Files;

public class LocalFileSystemFtpServiceTests : IDisposable
{
    private readonly string _remoteDirectory = Path.Combine(Path.GetTempPath(), "ovenbird-tests", Guid.NewGuid().ToString());
    private readonly LocalFileSystemFtpService _service = new();

    [Fact]
    public async Task ListAsync_ReturnsEmpty_WhenDirectoryDoesNotExist()
    {
        var ct = TestContext.Current.CancellationToken;

        var files = await _service.ListAsync(CreateOptions(), ct);

        files.ShouldBeEmpty();
    }

    [Fact]
    public async Task ListAsync_Then_DownloadAsync_RoundTripsFileContentFromDisk()
    {
        var ct = TestContext.Current.CancellationToken;

        Directory.CreateDirectory(_remoteDirectory);
        var expected = $"hello from local ftp {Guid.NewGuid()}";
        await File.WriteAllTextAsync(Path.Combine(_remoteDirectory, "file1.csv"), expected, ct);

        var options = CreateOptions();

        var files = await _service.ListAsync(options, ct);
        files.Count.ShouldBe(1);

        await using var stream = await _service.DownloadAsync(options, files[0], ct);
        using var reader = new StreamReader(stream);
        var actual = await reader.ReadToEndAsync(ct);

        actual.ShouldBe(expected);
    }

    [Fact]
    public async Task UploadAsync_WritesFileUnderRemoteDirectory()
    {
        var ct = TestContext.Current.CancellationToken;

        var options = CreateOptions();
        var expected = "uploaded content";

        await using (var content = new MemoryStream(Encoding.UTF8.GetBytes(expected)))
        {
            await _service.UploadAsync(options, content, "uploaded.csv", ct);
        }

        var writtenPath = Path.Combine(_remoteDirectory, "uploaded.csv");
        File.Exists(writtenPath).ShouldBeTrue();
        (await File.ReadAllTextAsync(writtenPath, ct)).ShouldBe(expected);
    }

    [Fact]
    public async Task DownloadAsync_ThrowsWhenRemotePathIsOutsideConfiguredDirectory()
    {
        var ct = TestContext.Current.CancellationToken;

        var options = CreateOptions();
        var outsidePath = Path.Combine(Path.GetTempPath(), "ovenbird-tests-outside.csv");

        await Should.ThrowAsync<InvalidOperationException>(
            () => _service.DownloadAsync(options, outsidePath, ct));
    }

    private FtpOptions CreateOptions() => new()
    {
        Host = "unused",
        Username = "unused",
        Password = "unused",
        RemoteDirectory = _remoteDirectory,
    };

    public void Dispose()
    {
        if (Directory.Exists(_remoteDirectory))
        {
            Directory.Delete(_remoteDirectory, recursive: true);
        }
    }
}
