using Core.Constants;
using Core.Market;
using Core.Market.Specifications;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments.Import;

public class ImportMarketDocumentHandler(
    IReadRepository<MarketDocument> readRepository,
    IRepository<MarketDocument> repository,
    IFtpService ftpService,
    IFileStorage fileStorage)
{
    public async Task<Result<long>> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var storageKey = $"market-documents/{DateTime.UtcNow:yyyy/MM/dd}/{Path.GetFileName(command.RemoteFilePath)}";
        var fileReference = fileStorage.BuildFileReference(storageKey);

        var existing = await readRepository.FirstOrDefaultAsync(new MarketDocumentByFileSpec(fileReference), ct);

        if (existing is not null)
        {
            return Result.Success(existing.Id);
        }

        using var ftpStream = await ftpService.DownloadAsync(command.RemoteFilePath, ct);

        var uploadedFileReference = await fileStorage.UploadAsync(ftpStream, storageKey, ct);

        var now = DateTimeOffset.UtcNow;

        var document = new MarketDocument
        {
            Name = Path.GetFileName(command.RemoteFilePath),
            File = uploadedFileReference,
            CompanyId = command.CompanyId,
            DirectionId = MarketDocumentDirections.Inbound,
            StatusId = MarketDocumentStatuses.New,
            CreatedAtUtc = now,
            LastModifiedAtUtc = now
        };

        var created = await repository.AddAsync(document, ct);

        return Result.Success(created.Id);
    }
}
