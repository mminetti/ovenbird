using System.Globalization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Core.Market;
using Core.Service;
using Core.Subscription;
using Core.Subscription.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.EntityBuilders;

public class HistoricalUsageEntityBuilder(
    IReadRepository<Account> accountReadRepository,
    IBigDataLookupService lookupService,
    IRepository<HistoricalUsage> historicalUsageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : IMarketDocumentEntityBuilder
{
    public const string Key = "867_02";

    private const string DateFormat = "yyyyMMdd";

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transactionElement = XElement.Parse(item.Raw);

        if (transactionElement.Element("Usage") is null)
        {
            throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no 'Usage' element.");
        }

        var transaction = Deserialize(transactionElement);
        var usage = transaction.Usage;

        var commodityId = lookupService.GetCommodityId(transaction.Commodity ?? string.Empty);
        var accountId = await GetAccountId(usage.TranNr814, ct);
        var servicePointId = await lookupService.GetServicePointId(usage.UtilityAccountNumber, ct);

        foreach (var meter in usage.UsageMeters)
        {
            var meterId = await lookupService.GetMeterId(meter.MeterIdentifier, ct);

            var periodStartDate = DateOnly.ParseExact(meter.ServicePeriodBeginDate, DateFormat, CultureInfo.InvariantCulture);
            var periodEndDate = DateOnly.ParseExact(meter.ServicePeriodEndDate, DateFormat, CultureInfo.InvariantCulture);

            foreach (var quantity in meter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var historicalUsage = new HistoricalUsage
                    {
                        Identifier = item.ReferenceNumber,
                        AccountId = accountId,
                        ServicePointId = servicePointId,
                        MeterId = meterId,
                        PeriodStartDate = periodStartDate,
                        PeriodEndDate = periodEndDate,
                        Consumption = read.ReadConsumption,
                        CommodityId = commodityId,
                        UnitOfMeasureId = lookupService.GetUnitOfMeasureId(read.ReadUOM),
                    };

                    var created = await historicalUsageRepository.AddAsync(historicalUsage, ct);

                    await referenceRepository.AddAsync(new MarketDocumentReference
                    {
                        MarketDocumentItemId = item.Id,
                        ReferenceTypeId = CoreConstants.MarketDocumentReferenceTypes.HistoricalUsage,
                        ReferenceId = created.Id,
                    }, ct);
                }
            }
        }
    }

    private static UsageTransactionXml Deserialize(XElement transactionElement)
    {
        var serializer = new XmlSerializer(typeof(UsageTransactionXml));

        using var reader = transactionElement.CreateReader();

        return (UsageTransactionXml)serializer.Deserialize(reader)!;
    }

    private async Task<long> GetAccountId(string? identifier, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new InvalidOperationException("Usage transaction is missing 'TranNr814'.");
        }

        // TODO: this should get the service start by identifier and return the account id 

        var account = await accountReadRepository.FirstOrDefaultAsync(new AccountByIdentifierSpec(identifier), ct)
            ?? throw new InvalidOperationException($"Account '{identifier}' was not found.");

        return account.Id;
    }
}
