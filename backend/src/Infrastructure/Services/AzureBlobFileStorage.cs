using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using UseCases.Interfaces.Files;

namespace Infrastructure.Services;

public class AzureBlobFileStorage : IFileStorage
{
    private readonly BlobContainerClient _containerClient;
    private readonly string _containerName;

    public AzureBlobFileStorage(IOptions<AzureBlobStorageOptions> options)
    {
        _containerName = options.Value.ContainerName;

        var serviceClient = new BlobServiceClient(options.Value.ConnectionString);
        _containerClient = serviceClient.GetBlobContainerClient(_containerName);
    }

    public string BuildFileReference(string storageKey) => $"{_containerName}/{storageKey}";

    public async Task<string> UploadAsync(Stream content, string storageKey, CancellationToken ct)
    {
        await _containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = _containerClient.GetBlobClient(storageKey);
        await blobClient.UploadAsync(content, overwrite: true, ct);

        return BuildFileReference(storageKey);
    }

    public async Task<Stream> OpenReadAsync(string fileReference, CancellationToken ct)
    {
        var blobClient = _containerClient.GetBlobClient(ExtractBlobName(fileReference));

        return await blobClient.OpenReadAsync(cancellationToken: ct);
    }

    private static string ExtractBlobName(string fileReference)
    {
        var separatorIndex = fileReference.IndexOf('/');

        return separatorIndex >= 0 ? fileReference[(separatorIndex + 1)..] : fileReference;
    }
}
