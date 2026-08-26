using FluentFTP;
using UseCases.Interfaces.Files;

namespace Infrastructure.Services.Files;

public class FluentFtpService() : IFtpService
{
    private static AsyncFtpClient CreateClient(FtpOptions options) =>
        new(options.Host, options.Username, options.Password, options.Port);

    public async Task<IReadOnlyList<string>> ListAsync(FtpOptions options, CancellationToken ct)
    {
        using var client = CreateClient(options);
        await client.Connect(ct);

        var listing = await client.GetListing(options.RemoteDirectory, ct);

        return listing
            .Where(item => item.Type == FtpObjectType.File)
            .Select(item => item.FullName)
            .ToList();
    }

    public async Task<Stream> DownloadAsync(FtpOptions options, string remotePath, CancellationToken ct)
    {
        using var client = CreateClient(options);
        await client.Connect(ct);

        var stream = new MemoryStream();
        await client.DownloadStream(stream, remotePath, token: ct);
        stream.Position = 0;

        return stream;
    }

    public async Task UploadAsync(FtpOptions options, Stream content, string remotePath, CancellationToken ct)
    {
        using var client = CreateClient(options);
        await client.Connect(ct);

        await client.UploadStream(content, remotePath, FtpRemoteExists.Overwrite, true, token: ct);
    }
}
