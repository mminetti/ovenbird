using Renci.SshNet;
using Renci.SshNet.Sftp;
using UseCases.Interfaces.Files;

namespace Infrastructure.Services.Files;

public class SshNetSftpService : ISftpService
{
    private static SftpClient CreateClient(FtpOptions options) =>
        new(options.Host, options.Port, options.Username, options.Password);

    public async Task<IReadOnlyList<string>> ListAsync(FtpOptions options, CancellationToken ct)
    {
        using var client = CreateClient(options);
        await client.ConnectAsync(ct);

        var files = new List<string>();

        await foreach (SftpFile file in client.ListDirectoryAsync(options.RemoteDirectory, ct))
        {
            if (file.IsRegularFile)
            {
                files.Add(file.FullName);
            }
        }

        return files;
    }

    public async Task<Stream> DownloadAsync(FtpOptions options, string remotePath, CancellationToken ct)
    {
        using var client = CreateClient(options);
        await client.ConnectAsync(ct);

        var stream = new MemoryStream();
        await client.DownloadFileAsync(remotePath, stream, ct);
        stream.Position = 0;

        return stream;
    }

    public async Task UploadAsync(FtpOptions options, Stream content, string remotePath, CancellationToken ct)
    {
        using var client = CreateClient(options);
        await client.ConnectAsync(ct);

        await client.UploadFileAsync(content, remotePath, canOverride: true, uploadProgress: null, ct);
    }
}
