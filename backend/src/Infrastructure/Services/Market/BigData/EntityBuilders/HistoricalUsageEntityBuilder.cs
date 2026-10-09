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

        var commodityId = lookupService.GetCommodityId(transaction.Commodity);
        var account = await GetAccountAsync(usage.TranNr814, ct);
        var servicePoint = await lookupService.GetServicePointAsync(usage.UtilityAccountNumber, ct);

        foreach (var usageMeter in usage.UsageMeters)
        {
            var meter = servicePoint.GetMeter(usageMeter.MeterIdentifier)
                ?? throw new InvalidOperationException($"Meter '{usageMeter.MeterIdentifier}' was not found.");

            var periodStartDate = DateOnly.ParseExact(usageMeter.ServicePeriodBeginDate, DateFormat, CultureInfo.InvariantCulture);
            var periodEndDate = DateOnly.ParseExact(usageMeter.ServicePeriodEndDate, DateFormat, CultureInfo.InvariantCulture);

            foreach (var quantity in usageMeter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var historicalUsage = new HistoricalUsage
                    {
                        Identifier = item.ReferenceNumber,
                        AccountId = account.Id,
                        ServicePointId = servicePoint.Id,
                        MeterId = meter.Id,
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

    private async Task<Account> GetAccountAsync(string? originalReferenceNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(originalReferenceNumber))
        {
            throw new InvalidOperationException("Usage transaction is missing original reference number.");
        }

        // TODO: this should get the service start by identifier and return the account id 

        var account = await accountReadRepository.FirstOrDefaultAsync(new AccountByIdentifierSpec(originalReferenceNumber), ct)
            ?? throw new InvalidOperationException($"Account '{originalReferenceNumber}' was not found.");

        return account;
    }
}
