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
    private readonly string _identifier = "edi.import";

    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var documentIds = new List<long>();
        var configurations = await configurationReadRepository.ListAsync(new ConfigurationByNameSpec(_identifier), ct);

        foreach (var configuration in configurations)
        {
            var company = configuration.Company
                ?? throw new InvalidOperationException($"Configuration '{configuration.Name}' has no company.");

            var import = strategyResolver.Resolve(configuration.ConfigurationType.Name);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(company.TimeZoneId);

            var remoteFilePaths = await import.ListFilesAsync(configuration, ct);

            foreach (var remoteFilePath in remoteFilePaths)
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

                var now = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), timeZone);
                var storageKey = $"market-documents/{company.Name}/{now:yyyy/MM/dd}/{fileName}";

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
        }

        return Result.Success<IReadOnlyList<long>>(documentIds);
    }
}
