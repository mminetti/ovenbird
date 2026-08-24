using Core.Constants;
using Core.Market;
using Core.Market.Specifications;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments.Import;

public class ImportMarketDocumentHandler(
    IReadRepository<MarketDocument> readRepository,
    IRepository<MarketDocument> repository,
    IFtpService ftpService,
    IFileStorage fileStorage,
    TimeProvider timeProvider)
{
    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var remoteFilePaths = await ftpService.ListFilesAsync(command.RemoteDirectory, ct);

        var documentIds = new List<long>();

        foreach (var remoteFilePath in remoteFilePaths)
        {
            var storageKey = $"market-documents/{timeProvider.GetUtcNow():yyyy/MM/dd}/{Path.GetFileName(remoteFilePath)}";
            var fileReference = fileStorage.BuildFileReference(storageKey);

            var existing = await readRepository.FirstOrDefaultAsync(new MarketDocumentByFileSpec(fileReference), ct);

            if (existing is not null)
            {
                documentIds.Add(existing.Id);
                continue;
            }

            using var ftpStream = await ftpService.DownloadAsync(remoteFilePath, ct);

            var uploadedFileReference = await fileStorage.UploadAsync(ftpStream, storageKey, ct);

            var now = timeProvider.GetUtcNow();

            var document = new MarketDocument
            {
                Name = Path.GetFileName(remoteFilePath),
                File = uploadedFileReference,
                CompanyId = command.CompanyId,
                DirectionId = MarketDocumentDirections.Inbound,
                StatusId = MarketDocumentStatuses.New
            };

            var created = await repository.AddAsync(document, ct);

            documentIds.Add(created.Id);
        }

        return Result.Success<IReadOnlyList<long>>(documentIds);
    }
}
