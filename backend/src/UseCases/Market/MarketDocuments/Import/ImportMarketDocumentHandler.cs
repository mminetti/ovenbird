using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Interfaces.Files;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Import;

public record ImportMarketDocumentCommand();

public class ImportMarketDocumentHandler(
    IRepository<MarketDocument> documentRepository,
    IReadRepository<MarketDocument> documentReadRepository,
    IReadRepository<Configuration> configurationReadRepository,
    IConnectorFtpTransport ftpTransport,
    IConnectorFileStorageTransport fileStorageTransport,
    IMessageBus bus)
{
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

                var remoteFilePaths = await ftpTransport.ListFilesAsync(configuration, ct);

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

                        using var fileStream = await ftpTransport.DownloadFileAsync(configuration, remoteFilePath, ct);

                        var uploadedFileReference = await fileStorageTransport.UploadAsync(configuration, fileStream, fileName, ct);

                        var document = new MarketDocument
                        {
                            Name = fileName,
                            File = uploadedFileReference,
                            CompanyId = company.Id,
                            DirectionId = CoreConstants.MarketDocumentDirections.Inbound,
                            StatusId = CoreConstants.MarketDocumentStatuses.New
                        };

                        var created = await documentRepository.AddAsync(document, ct);

                        _ = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(
                            new ImportMarketDocumentItemCommand(configuration.Id, created.Id), ct);

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
}
