using System.Globalization;
using Core.MarketExchange;
using Core.ServiceDelivery;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public abstract class UsageTransactionImportBuilderBase<TUsage>(
    IBigDataValueMapper valueMapper,
    IRepository<TUsage> usageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<UsageTransactionXml>, IMarketDocumentEntityBuilder
    where TUsage : UsageBase
{
    protected override string RequiredElementName => "Usage";

    protected IBigDataValueMapper ValueMapper => valueMapper;

    protected abstract int ReferenceTypeId { get; }

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var usage = transaction.Usage;

        var commodityId = valueMapper.GetCommodityId(transaction.Commodity);
        var servicePoint = await GetServicePointAsync(usage.UtilityAccountNumber, ct);

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
                    usageEntity.ServicePointId = servicePoint.Id;
                    usageEntity.MeterId = meter.Id;
                    usageEntity.PeriodStartDate = periodStartDate;
                    usageEntity.PeriodEndDate = periodEndDate;
                    usageEntity.Consumption = read.ReadConsumption;
                    usageEntity.CommodityId = commodityId;
                    usageEntity.UnitOfMeasureId = valueMapper.GetUnitOfMeasureId(read.ReadUOM);

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

    /// <summary>Resolves the ServicePoint (with its meters) for the transaction's utility account number.</summary>
    protected abstract Task<ServicePoint> GetServicePointAsync(string identifier, CancellationToken ct);

    /// <summary>Creates the entity for a single meter read, populated with its type-specific fields. Shared fields are filled in by the base class.</summary>
    protected abstract TUsage CreateUsage(UsageReadXml read);
}
