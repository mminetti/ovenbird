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
    IReadRepository<Company> companyReadRepository,
    IReadRepository<SystemIntegration> integrationReadRepository,
    MarketImportStrategyResolver strategyResolver,
    TimeProvider timeProvider)
{
    private readonly string _identifier = "edi.import";
    private readonly string _handlerIdentifier = "edi.import.handler";

    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var documentIds = new List<long>();
        var companies = await companyReadRepository.ListAsync(new CompanyWithMarketSpec(), ct);
        var integrations = await integrationReadRepository.ListAsync(ct);

        foreach (var company in companies)
        {
            var integration = GetRequiredIntegration(integrations, company.Id, _identifier);

            var identifier = integration.GetRequiredValue(_handlerIdentifier);

            var import = strategyResolver.Resolve(identifier);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(company.Market.TimeZoneId);

            var remoteFilePaths = await import.ListFilesAsync(integration, ct);

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

                using var ftpStream = await import.DownloadFileAsync(integration, remoteFilePath, ct);

                var now = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), timeZone);
                var storageKey = $"market-documents/{company.Name}/{now:yyyy/MM/dd}/{fileName}";

                var uploadedFileReference = await import.UploadDocumentAsync(integration, ftpStream, storageKey, ct);

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

    private static SystemIntegration GetRequiredIntegration(
        IList<SystemIntegration> integrations, int companyId, string identifier)
    {
        return integrations.FirstOrDefault(x => x.Identifier == identifier &&
                (x.CompanyId == companyId || x.CompanyId is null))
                ?? throw new InvalidOperationException($"Integration {identifier} for company {companyId} not found");
    }
}
