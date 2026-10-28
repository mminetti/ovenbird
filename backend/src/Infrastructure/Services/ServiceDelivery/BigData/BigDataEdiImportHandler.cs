using Core.Assets;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData;

public class BigDataEdiImportHandler(
    IConnectorFtpTransport ftpTransport,
    IConnectorFileStorageTransport fileStorageTransport,
    IReadRepository<MarketDocument> documentReadRepository,
    MarketDocumentParserResolver parserResolver)
    : IEdiImportHandler
{
    public string Identifier => "BigData";

    public async Task<EdiImportResult> ExecuteImportAsync(
        Configuration configuration,
        CancellationToken ct)
    {
        var documents = new List<MarketDocument>();
        var errors = new List<ValidationError>();

        try
        {
            var company = configuration.GetRequiredCompany();
            var remoteFiles = await ftpTransport.ListFilesAsync(configuration, ct);

            foreach (var remoteFile in remoteFiles)
            {
                var identifier = Path.GetFileName(remoteFile);

                try
                {
                    var existing = await documentReadRepository.FirstOrDefaultAsync(
                        new MarketDocumentByIdentifierAndCompanySpec(identifier, company.Id), ct);

                    if (existing is not null)
                    {
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
                catch (Exception ex)
                {
                    errors.Add(new ValidationError(identifier, ex.Message, nameof(MarketDocument), ValidationSeverity.Error));
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ValidationError(configuration.Name, ex.Message, nameof(Configuration), ValidationSeverity.Error));
        }

        return new EdiImportResult(documents, errors);
    }

    public async Task<IReadOnlyList<MarketDocumentItem>> ExecuteItemImportAsync(
        Configuration configuration, 
        MarketDocument document, 
        CancellationToken ct)
    {
        var reference = document.Asset?.Reference
            ?? throw new InvalidOperationException($"MarketDocument '{document.Id}' has no associated asset.");

        using var content = await fileStorageTransport.OpenReadAsync(configuration, reference, ct);

        return await parserResolver.Resolve(Identifier).ReadTransactionsAsync(content, ct);
    }
}
