using Core.Common.Extensions;
using Core.Constants;
using Core.Market;
using Core.Market.Specifications;
using Core.Shared;
using Core.Shared.Specifications;
using UseCases.Market.MarketDocuments.Import.Strategies;

namespace UseCases.Market.MarketDocuments.Import;

public class ImportMarketDocumentHandler(
    IRepository<MarketDocument> documentRepository,
    IReadRepository<MarketDocument> documentReadRepository,
    IReadRepository<Configuration> configurationReadRepository,
    MarketImportStrategyResolver strategyResolver,
    TimeProvider timeProvider)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var documentIds = new List<long>();
        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeNameSpec(ConfigurationTypes.EdiImport), ct);

        foreach (var configuration in configurations)
        {
            try
            {
                var company = configuration.GetRequiredCompany();

                var import = strategyResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(company.TimeZoneId);

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

                        using var ftpStream = await import.DownloadFileAsync(configuration, remoteFilePath, ct);

                        var storageKey = GetStorageKey(timeProvider.GetUtcNow(), timeZone, company.Name, fileName);

                        var uploadedFileReference = await import.UploadDocumentAsync(configuration, ftpStream, storageKey, ct);

                        var document = new MarketDocument
                        {
                            Name = fileName,
                            File = uploadedFileReference,
                            CompanyId = company.Id,
                            DirectionId = MarketDocumentDirections.Inbound,
                            StatusId = MarketDocumentStatuses.New
                        };

                        var created = await documentRepository.AddAsync(document, ct);

                        documentIds.Add(created.Id);
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

    private static string GetStorageKey(DateTimeOffset utcNow, TimeZoneInfo timeZone, string companyName, string fileName)
    {
        var now = TimeZoneInfo.ConvertTime(utcNow, timeZone);
        var rootFilePath = $"edi/import/{companyName}/{now:yyyy/MM/dd}".ToSlug();
        return $"{rootFilePath}/{fileName}";
    }
}
