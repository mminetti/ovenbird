using Core.Constants;
using Core.Market;
using Core.Market.Specifications;
using Core.Shared;
using Core.Shared.Specifications;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments.Import;

public class ImportMarketDocumentHandler(
    IReadRepository<MarketDocument> readRepository,
    IRepository<MarketDocument> repository,
    IReadRepository<Company> companyRepository,
    MarketConnectionStrategyResolver strategyResolver,
    IFileStorage fileStorage,
    TimeProvider timeProvider)
{
    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var documentIds = new List<long>();
        var companies = await companyRepository.ListAsync(new CompanyWithMarketSpec(), ct);

        foreach (var company in companies)
        {
            var connection = strategyResolver.Resolve(company.Market.Identifier);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(company.Market.TimeZoneId);

            var remoteFilePaths = await connection.ListFilesAsync(company, command.RemoteDirectory, ct);

            foreach (var remoteFilePath in remoteFilePaths)
            {
                var fileName = Path.GetFileName(remoteFilePath);

                var existing = await readRepository.FirstOrDefaultAsync(
                    new MarketDocumentByNameAndCompanySpec(fileName, company.Id), ct);

                if (existing is not null)
                {
                    documentIds.Add(existing.Id);
                    continue;
                }

                using var ftpStream = await connection.DownloadFileAsync(company, remoteFilePath, ct);

                var now = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), timeZone);
                var storageKey = $"market-documents/{company.Name}/{now:yyyy/MM/dd}/{fileName}";

                var uploadedFileReference = await fileStorage.UploadAsync(ftpStream, storageKey, ct);

                var document = new MarketDocument
                {
                    Name = fileName,
                    File = uploadedFileReference,
                    CompanyId = company.Id,
                    DirectionId = MarketDocumentDirections.Inbound,
                    StatusId = MarketDocumentStatuses.New
                };

                var created = await repository.AddAsync(document, ct);

                documentIds.Add(created.Id);
            }
        }

        return Result.Success<IReadOnlyList<long>>(documentIds);
    }
}
