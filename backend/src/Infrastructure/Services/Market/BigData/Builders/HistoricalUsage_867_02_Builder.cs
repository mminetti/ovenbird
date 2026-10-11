using Core.Market;
using Core.Service;
using Core.Subscription;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.Builders;

public class HistoricalUsage_867_02_Builder(
    IReadRepository<Account> accountReadRepository,
    IBigDataLookupService lookupService,
    IRepository<HistoricalUsage> historicalUsageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : UsageTransactionBuilderBase<HistoricalUsage>(accountReadRepository, lookupService, historicalUsageRepository, referenceRepository)
{
    public const string Key = "867_02";

    protected override int ReferenceTypeId => CoreConstants.MarketDocumentReferenceTypes.HistoricalUsage;

    protected override HistoricalUsage CreateUsage(UsageReadXml read) => new();
}
