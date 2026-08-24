namespace UseCases.Interfaces.Files;

public interface IFileStorage
{
    Task<string> UploadAsync(Stream content, string storageKey, CancellationToken ct);
    Task<Stream> OpenReadAsync(string fileReference, CancellationToken ct);
}
