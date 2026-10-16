using Core.Settings;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments.Export.Interfaces;

namespace Infrastructure.Services.Market.BigData;

public class BigDataEdiExportHandler(
    IConnectorFtpTransport ftpTransport,
    IConnectorFileStorageTransport fileStorageTransport)
    : IEdiExportHandler
{
    public string Identifier => "BigData";

    public async Task<string> StoreTransactionAsync(Configuration configuration, Stream content, string fileName, CancellationToken ct)
    {
        var fileReference = await fileStorageTransport.UploadAsync(configuration, content, fileName, ct);

        content.Position = 0;
        await ftpTransport.UploadFileAsync(configuration, content, fileName, ct);

        return fileReference;
    }
}
