using System.Globalization;
using System.Xml.Linq;
using System.Xml.Serialization;
using Core.Market;
using Core.Service;
using Core.Subscription;
using Core.Subscription.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public abstract class UsageTransactionBuilderBase<TUsage>(
    IReadRepository<Account> accountReadRepository,
    IBigDataLookupService lookupService,
    IRepository<TUsage> usageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : IMarketDocumentEntityBuilder
    where TUsage : UsageBase
{
    private const string DateFormat = "yyyyMMdd";

    protected IBigDataLookupService LookupService => lookupService;

    protected abstract int ReferenceTypeId { get; }

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var usage = transaction.Usage;

        var commodityId = lookupService.GetCommodityId(transaction.Commodity);
        var account = await GetAccountAsync(usage.TranNr814, ct);
        var servicePoint = await lookupService.GetServicePointAsync(usage.UtilityAccountNumber, ct);

        await PrepareAsync(usage, ct);

        foreach (var usageMeter in SelectUsageMeters(usage.UsageMeters))
        {
            var meter = servicePoint.GetMeter(usageMeter.MeterIdentifier)
                ?? throw new InvalidOperationException($"Meter '{usageMeter.MeterIdentifier}' was not found.");

            var periodStartDate = DateOnly.ParseExact(usageMeter.ServicePeriodBeginDate, DateFormat, CultureInfo.InvariantCulture);
            var periodEndDate = DateOnly.ParseExact(usageMeter.ServicePeriodEndDate, DateFormat, CultureInfo.InvariantCulture);

            foreach (var quantity in usageMeter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var usageEntity = CreateUsage(read);
                    usageEntity.Identifier = item.ReferenceNumber;
                    usageEntity.AccountId = account.Id;
                    usageEntity.ServicePointId = servicePoint.Id;
                    usageEntity.MeterId = meter.Id;
                    usageEntity.PeriodStartDate = periodStartDate;
                    usageEntity.PeriodEndDate = periodEndDate;
                    usageEntity.Consumption = read.ReadConsumption;
                    usageEntity.CommodityId = commodityId;
                    usageEntity.UnitOfMeasureId = lookupService.GetUnitOfMeasureId(read.ReadUOM);

                    var created = await usageRepository.AddAsync(usageEntity, ct);

                    var reference = new MarketDocumentReference
                    {
                        MarketDocumentItemId = item.Id,
                        ReferenceTypeId = ReferenceTypeId,
                        ReferenceId = created.Id,
                    };

                    await referenceRepository.AddAsync(reference, ct);
                }
            }
        }
    }

    /// <summary>Selects which usage meters produce entities for this transaction type. Defaults to all of them.</summary>
    protected virtual IEnumerable<UsageMeterXml> SelectUsageMeters(IEnumerable<UsageMeterXml> usageMeters) => usageMeters;

    /// <summary>Resolves any transaction-level context needed by <see cref="CreateUsage"/>, once per transaction.</summary>
    protected virtual Task PrepareAsync(UsageXml usage, CancellationToken ct) => Task.CompletedTask;

    /// <summary>Creates the entity for a single meter read, populated with its type-specific fields. Shared fields are filled in by the base class.</summary>
    protected abstract TUsage CreateUsage(UsageReadXml read);

    private static UsageTransactionXml DeserializeTransaction(MarketDocumentItem item)
    {
        var transactionElement = XElement.Parse(item.Raw);

        if (transactionElement.Element("Usage") is null)
        {
            throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no 'Usage' element.");
        }

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
