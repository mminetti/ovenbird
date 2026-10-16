using Core.Settings;

namespace UseCases.Interfaces.Files;

public interface IConnectorFileStorageTransport
{
    Task<string> UploadAsync(Configuration configuration, Stream content, string fileName, CancellationToken ct);
    Task<Stream> OpenReadAsync(Configuration configuration, string fileReference, CancellationToken ct);
}
