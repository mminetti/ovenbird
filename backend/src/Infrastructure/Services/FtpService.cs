using FluentFTP;
using Microsoft.Extensions.Options;
using UseCases.Interfaces.Files;

namespace Infrastructure.Services;

public class FtpService(IOptions<FtpOptions> options) : IFtpService
{
    private readonly FtpOptions _options = options.Value;

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        await using var client = new AsyncFtpClient(_options.Host, _options.Username, _options.Password, _options.Port);
        await client.Connect(ct);

        var stream = new MemoryStream();
        await client.DownloadStream(stream, remotePath, token: ct);
        stream.Position = 0;

        return stream;
    }

    public async Task UploadAsync(Stream content, string remotePath, CancellationToken ct)
    {
        await using var client = new AsyncFtpClient(_options.Host, _options.Username, _options.Password, _options.Port);
        await client.Connect(ct);

        await client.UploadStream(content, remotePath, FtpRemoteExists.Overwrite, true, token: ct);
    }
}
