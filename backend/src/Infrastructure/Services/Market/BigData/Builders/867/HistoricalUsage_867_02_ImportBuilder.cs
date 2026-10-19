using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;

namespace Infrastructure.Services.Market.BigData.Builders;

public class HistoricalUsage_867_02_ImportBuilder(
    IReadRepository<ServicePoint> servicePointReadRepository,
    IBigDataValueMapper valueMapper,
    IRepository<HistoricalUsage> historicalUsageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : UsageTransactionImportBuilderBase<HistoricalUsage>(valueMapper, historicalUsageRepository, referenceRepository)
{
    public const string Key = "BigData_867_02";

    protected override int ReferenceTypeId => Constants.MarketDocumentReferenceTypes.HistoricalUsage;

    protected override HistoricalUsage CreateUsage(UsageReadXml read) => new();

    protected override async Task<ServicePoint> GetServicePointAsync(string identifier, CancellationToken ct) =>
        await servicePointReadRepository.FirstOrDefaultAsync(new ServicePointWithMetersByIdentifierSpec(identifier), ct)
            ?? throw new InvalidOperationException($"Service Point '{identifier}' was not found.");
}
