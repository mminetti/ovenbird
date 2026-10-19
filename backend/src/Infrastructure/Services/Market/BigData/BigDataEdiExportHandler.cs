using Core.Assets;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments.Export.Interfaces;

namespace Infrastructure.Services.Market.BigData;

public class BigDataEdiExportHandler(
    IConnectorFtpTransport ftpTransport,
    IConnectorFileStorageTransport fileStorageTransport,
    IReadRepository<MarketDocumentItem> itemReadRepository,
    MarketDocumentGeneratorResolver generatorResolver,
    TimeProvider timeProvider)
    : IEdiExportHandler
{
    private const string Set = "814";

    public string Identifier => "BigData";

    public async Task<MarketDocumentExportResult?> ExecuteExportAsync(Configuration configuration, CancellationToken ct)
    {
        var pendingItems = await itemReadRepository.ListAsync(new MarketDocumentItemsPendingExportSpec(Set), ct);

        if (pendingItems.Count == 0)
        {
            return null;
        }

        var company = configuration.GetRequiredCompany();
        var generator = generatorResolver.Resolve(Identifier);

        var now = company.GetLocalNow(timeProvider);
        var fileName = $"{configuration.Name}_{now:yyyyMMddHHmmss}.xml";

        using var content = await generator.WriteTransactionsAsync(pendingItems, ct);

        var fileReference = await fileStorageTransport.UploadAsync(configuration, content, fileName, ct);

        content.Position = 0;
        await ftpTransport.UploadFileAsync(configuration, content, fileName, ct);

        var document = new MarketDocument
        {
            Identifier = fileName,
            CompanyId = company.Id,
            DirectionId = Constants.MarketDocumentDirections.Outbound,
            StatusId = Constants.MarketDocumentStatuses.New,
            Asset = new Asset
            {
                FileName = fileName,
                Extension = Path.GetExtension(fileName),
                Reference = fileReference
            }
        };

        return new MarketDocumentExportResult(document, pendingItems);
    }
}
