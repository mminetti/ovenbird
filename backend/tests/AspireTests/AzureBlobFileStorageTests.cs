using System.Text;
using Aspire.Hosting;
using Infrastructure.Services.Files;
using UseCases.Interfaces.Files;

namespace AspireTests;

public class AzureBlobFileStorageTests
{
    [Fact]
    public async Task UploadAsync_Then_OpenReadAsync_RoundTripsContent_AgainstAzuriteEmulator()
    {
        var ct = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireHost>(cancellationToken: ct);

        // AppHost.cs pins "sqlserver" to a persistent container lifetime for fast local dev
        // iteration. Force every container back to Session here so disposing the app actually
        // stops it instead of leaking it for tests, which don't need it kept around.
        foreach (var containerResource in appHost.Resources.OfType<ContainerResource>())
        {
            appHost.CreateResourceBuilder(containerResource).WithLifetime(ContainerLifetime.Session);
        }

        await using var app = await appHost.BuildAsync(ct);

        await app.StartAsync(ct);

        await app.ResourceNotifications.WaitForResourceHealthyAsync("blobs", ct)
            .WaitAsync(TimeSpan.FromSeconds(60), ct);

        var connectionString = await app.GetConnectionStringAsync("blobs", ct);
        Assert.False(string.IsNullOrEmpty(connectionString));

        var options = new FileStorageOptions
        {
            ConnectionString = connectionString!,
            RootDirectory = "import-market-documents",
        };

        var storage = new AzureBlobFileStorage();
        var expected = $"hello from azurite {Guid.NewGuid()}";
        var remotePath = $"{Guid.NewGuid()}.txt";

        await using (var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(expected)))
        {
            var reference = await storage.UploadAsync(options, uploadStream, remotePath, ct);

            await using var downloadStream = await storage.OpenReadAsync(options, reference, ct);
            using var reader = new StreamReader(downloadStream);
            var actual = await reader.ReadToEndAsync(ct);

            Assert.Equal(expected, actual);
        }
    }
}
