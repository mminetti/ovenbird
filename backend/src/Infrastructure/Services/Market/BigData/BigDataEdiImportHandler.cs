using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData;

public class BigDataEdiImportHandler(
    IConnectorFtpTransport ftpTransport,
    IConnectorFileStorageTransport fileStorageTransport,
    IReadRepository<MarketDocument> documentReadRepository)
    : IEdiImportHandler
{
    public string Identifier => "BigData";

    public Task<IReadOnlyList<string>> ListTransactionsAsync(Configuration configuration, CancellationToken ct)
    { 
        return ftpTransport.ListFilesAsync(configuration, ct);
    }

    public async Task<MarketDocument?> FindExistingTransactionAsync(Configuration configuration, string reference, CancellationToken ct)
    {
        var company = configuration.GetRequiredCompany();
        var name = Path.GetFileName(reference);

        return await documentReadRepository.FirstOrDefaultAsync(
            new MarketDocumentByNameAndCompanySpec(name, company.Id), ct);
    }

    public async Task<TransactionReference> StoreTransactionAsync(Configuration configuration, string reference, CancellationToken ct)
    {
        var name = Path.GetFileName(reference);

        using var content = await ftpTransport.DownloadFileAsync(configuration, reference, ct);
        var fileReference = await fileStorageTransport.UploadAsync(configuration, content, name, ct);

        return new TransactionReference(name, fileReference);
    }

    public Task<Stream> OpenTransactionAsync(Configuration configuration, string reference, CancellationToken ct)
    { 
        return fileStorageTransport.OpenReadAsync(configuration, reference, ct);
    }
}
