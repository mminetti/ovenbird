using Core.Market;
using Core.Market.Specifications;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Strategies;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Import;

public class ImportMarketDocumentHandler(
    IRepository<MarketDocument> documentRepository,
    IReadRepository<MarketDocument> documentReadRepository,
    IReadRepository<Configuration> configurationReadRepository,
    MarketImportStrategyResolver strategyResolver)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var documentIds = new List<long>();

        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeSpec(CoreConstants.ConfigurationTypes.EdiImport), ct);

        foreach (var configuration in configurations)
        {
            try
            {
                var company = configuration.GetRequiredCompany();

                var import = strategyResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

                var remoteFilePaths = await import.ListFilesAsync(configuration, ct);

                foreach (var remoteFilePath in remoteFilePaths)
                {
                    try
                    {
                        var fileName = Path.GetFileName(remoteFilePath);

                        var existing = await documentReadRepository.FirstOrDefaultAsync(
                            new MarketDocumentByNameAndCompanySpec(fileName, company.Id), ct);

                        if (existing is not null)
                        {
                            documentIds.Add(existing.Id);
                            continue;
                        }

                        using var fileStream = await import.DownloadFileAsync(configuration, remoteFilePath, ct);

                        var uploadedFileReference = await import.UploadDocumentAsync(configuration, fileStream, fileName, ct);

                        var document = new MarketDocument
                        {
                            Name = fileName,
                            File = uploadedFileReference,
                            CompanyId = company.Id,
                            DirectionId = CoreConstants.MarketDocumentDirections.Inbound,
                            StatusId = CoreConstants.MarketDocumentStatuses.New
                        };

                        var created = await documentRepository.AddAsync(document, ct);

                        documentIds.Add(created.Id);

                        // publish event market.document.inbound.created
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        return Result.Success<IReadOnlyList<long>>(documentIds);
    }
}
