using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using UseCases.Interfaces.Files;

namespace Infrastructure.Services.Files;

public class SshNetSftpService(IOptions<SftpOptions> options) : ISftpService
{
    private readonly SftpOptions _options = options.Value;

    private SftpClient CreateClient() => new(_options.Host, _options.Port, _options.Username, _options.Password);

    public async Task<IReadOnlyList<string>> ListAsync(string remoteDirectory, CancellationToken ct)
    {
        using var client = CreateClient();
        await client.ConnectAsync(ct);

        var files = new List<string>();

        await foreach (SftpFile file in client.ListDirectoryAsync(remoteDirectory, ct))
        {
            if (file.IsRegularFile)
            {
                files.Add(file.FullName);
            }
        }

        return files;
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        using var client = CreateClient();
        await client.ConnectAsync(ct);

        var stream = new MemoryStream();
        await client.DownloadFileAsync(remotePath, stream, ct);
        stream.Position = 0;

        return stream;
    }

    public async Task UploadAsync(Stream content, string remotePath, CancellationToken ct)
    {
        using var client = CreateClient();
        await client.ConnectAsync(ct);

        await client.UploadFileAsync(content, remotePath, canOverride: true, uploadProgress: null, ct);
    }
}
