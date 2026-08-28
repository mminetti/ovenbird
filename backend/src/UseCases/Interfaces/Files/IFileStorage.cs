namespace UseCases.Interfaces.Files;

public interface IFileStorage
{
    Task<string> UploadAsync(FileStorageOptions options, Stream content, string remotePath, CancellationToken ct);
    Task<Stream> OpenReadAsync(FileStorageOptions options, string remotePath, CancellationToken ct);
}
