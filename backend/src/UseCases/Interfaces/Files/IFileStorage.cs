namespace UseCases.Interfaces.Files;

public interface IFileStorage
{
    string BuildFileReference(string storageKey);
    Task<string> UploadAsync(Stream content, string storageKey, CancellationToken ct);
    Task<Stream> OpenReadAsync(string fileReference, CancellationToken ct);
}
