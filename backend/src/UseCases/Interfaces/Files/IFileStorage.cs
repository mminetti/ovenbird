namespace UseCases.Interfaces.Files;

public interface IFileStorage
{
    Task<string> UploadAsync(FileOptions options, Stream content, string remotePath, CancellationToken ct);
    Task<Stream> OpenReadAsync(FileOptions options, string remotePath, CancellationToken ct);
}
