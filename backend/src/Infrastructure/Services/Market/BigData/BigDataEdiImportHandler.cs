using Core.Assets;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData;

public class BigDataEdiImportHandler(
    IConnectorFtpTransport ftpTransport,
    IConnectorFileStorageTransport fileStorageTransport,
    IReadRepository<MarketDocument> documentReadRepository,
    MarketDocumentParserResolver parserResolver)
    : IEdiImportHandler
{
    public string Identifier => "BigData";

    public async Task<IReadOnlyList<MarketDocument>> ExecuteImportAsync(Configuration configuration, CancellationToken ct)
    {
        var company = configuration.GetRequiredCompany();
        var remoteFiles = await ftpTransport.ListFilesAsync(configuration, ct);

        var documents = new List<MarketDocument>();

        foreach (var remoteFile in remoteFiles)
        {
            var identifier = Path.GetFileName(remoteFile);

            var existing = await documentReadRepository.FirstOrDefaultAsync(
                new MarketDocumentByIdentifierAndCompanySpec(identifier, company.Id), ct);

            if (existing is not null)
            {
                documents.Add(existing);
                continue;
            }

            using var content = await ftpTransport.DownloadFileAsync(configuration, remoteFile, ct);

            var fileReference = await fileStorageTransport.UploadAsync(configuration, content, identifier, ct);

            documents.Add(new MarketDocument
            {
                Identifier = identifier,
                CompanyId = company.Id,
                DirectionId = Constants.MarketDocumentDirections.Inbound,
                StatusId = Constants.MarketDocumentStatuses.New,
                Asset = new Asset
                {
                    FileName = identifier,
                    Extension = Path.GetExtension(identifier),
                    Reference = fileReference
                }
            });
        }

        return documents;
    }

    public async Task<IReadOnlyList<MarketDocumentItem>> ExecuteItemImportAsync(
        Configuration configuration, MarketDocument document, CancellationToken ct)
    {
        var reference = document.Asset?.Reference
            ?? throw new InvalidOperationException($"MarketDocument '{document.Id}' has no associated asset.");

        using var content = await fileStorageTransport.OpenReadAsync(configuration, reference, ct);

        return await parserResolver.Resolve(Identifier).ReadTransactionsAsync(content, ct);
    }
}
