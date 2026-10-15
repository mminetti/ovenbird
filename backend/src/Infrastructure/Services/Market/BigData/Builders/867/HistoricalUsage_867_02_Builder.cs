using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;

namespace Infrastructure.Services.Market.BigData.Builders;

public class HistoricalUsage_867_02_Builder(
    IBigDataLookupService lookupService,
    IRepository<HistoricalUsage> historicalUsageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : UsageTransactionBuilderBase<HistoricalUsage>(lookupService, historicalUsageRepository, referenceRepository)
{
    public const string Key = "867_02";

    protected override int ReferenceTypeId => Constants.MarketDocumentReferenceTypes.HistoricalUsage;

    protected override HistoricalUsage CreateUsage(UsageReadXml read) => new();
}
