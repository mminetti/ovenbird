using Azure.Storage.Blobs;
using UseCases.Interfaces.Files;

namespace Infrastructure.Services.Files;

public class AzureBlobFileStorage : IFileStorage
{
    private static BlobContainerClient GetClient(UseCases.Interfaces.Files.FileOptions options)
    {
        var serviceClient = new BlobServiceClient(options.ConnectionString);
        return serviceClient.GetBlobContainerClient(options.RootDirectory);
    }

    public async Task<string> UploadAsync(
        UseCases.Interfaces.Files.FileOptions options, 
        Stream content, 
        string remotePath, 
        CancellationToken ct)
    {
        var client = GetClient(options);

        await client.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = client.GetBlobClient(remotePath);
        await blobClient.UploadAsync(content, overwrite: true, ct);

        return $"{options.RootDirectory}/{remotePath}";
    }

    public async Task<Stream> OpenReadAsync(
        UseCases.Interfaces.Files.FileOptions options, 
        string remotePath, 
        CancellationToken ct)
    {
        var client = GetClient(options);

        var blobClient = client.GetBlobClient(ExtractBlobName(remotePath));

        return await blobClient.OpenReadAsync(cancellationToken: ct);
    }

    private static string ExtractBlobName(string fileReference)
    {
        var separatorIndex = fileReference.IndexOf('/');

        return separatorIndex >= 0 ? fileReference[(separatorIndex + 1)..] : fileReference;
    }
}
