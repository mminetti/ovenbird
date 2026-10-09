using System.Globalization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Core.Market;
using Core.Service;
using Core.Service.Specifications;
using Core.Subscription;
using Core.Subscription.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.EntityBuilders;

public class HistoricalUsageEntityBuilder(
    IReadRepository<Account> accountReadRepository,
    IReadRepository<ServicePoint> servicePointReadRepository,
    IReadRepository<Meter> meterReadRepository,
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
        var commodityId = EdiCodeMappings.ResolveCommodityId(transaction.Commodity ?? string.Empty);

        var usage = transaction.Usage;

        var accountId = await ResolveAccountId(usage.UtilityAccountNumber, ct);
        var servicePointId = await ResolveServicePointId(item.ServicePointIdentifier, ct);

        var readIndex = 0;

        foreach (var meter in usage.UsageMeters)
        {
            var meterId = await ResolveMeterId(meter.MeterIdentifier, ct);

            var periodStartDate = DateOnly.ParseExact(meter.ServicePeriodBeginDate, DateFormat, CultureInfo.InvariantCulture);
            var periodEndDate = DateOnly.ParseExact(meter.ServicePeriodEndDate, DateFormat, CultureInfo.InvariantCulture);

            foreach (var quantity in meter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var historicalUsage = new HistoricalUsage
                    {
                        Identifier = $"{item.ReferenceNumber}-{readIndex++}",
                        AccountId = accountId,
                        ServicePointId = servicePointId,
                        MeterId = meterId,
                        PeriodStartDate = periodStartDate,
                        PeriodEndDate = periodEndDate,
                        Consumption = read.ReadConsumption,
                        CommodityId = commodityId,
                        UnitOfMeasureId = EdiCodeMappings.ResolveUnitOfMeasureId(read.ReadUOM),
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

    private async Task<long> ResolveAccountId(string? identifier, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new InvalidOperationException("Usage transaction is missing 'UtilityAccountNumber'.");
        }

        var account = await accountReadRepository.FirstOrDefaultAsync(new AccountByIdentifierSpec(identifier), ct)
            ?? throw new InvalidOperationException($"Account '{identifier}' was not found.");

        return account.Id;
    }

    private async Task<long> ResolveServicePointId(string? identifier, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new InvalidOperationException("Usage transaction is missing its ServicePoint identifier.");
        }

        var servicePoint = await servicePointReadRepository.FirstOrDefaultAsync(new ServicePointByIdentifierSpec(identifier), ct)
            ?? throw new InvalidOperationException($"ServicePoint '{identifier}' was not found.");

        return servicePoint.Id;
    }

    private async Task<long?> ResolveMeterId(string? identifier, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return null;
        }

        var meter = await meterReadRepository.FirstOrDefaultAsync(new MeterByIdentifierSpec(identifier), ct)
            ?? throw new InvalidOperationException($"Meter '{identifier}' was not found.");

        return meter.Id;
    }
}
