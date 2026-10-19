using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;

namespace Infrastructure.Services.Market.BigData.Builders;

public class HistoricalUsage_867_02_ImportBuilder(
    IBigDataLookupService lookupService,
    IRepository<HistoricalUsage> historicalUsageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : UsageTransactionImportBuilderBase<HistoricalUsage>(lookupService, historicalUsageRepository, referenceRepository)
{
    public const string Key = "BigData_867_02";

    protected override int ReferenceTypeId => Constants.MarketDocumentReferenceTypes.HistoricalUsage;

    protected override HistoricalUsage CreateUsage(UsageReadXml read) => new();
}
