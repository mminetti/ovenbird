using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Core.Subscription;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public class Usage_867_03_ImportBuilder(
    IReadRepository<Usage> usageReadRepository,
    IBigDataValueMapper valueMapper,
    IRepository<Usage> usageRepository,
    IRepository<MarketDocumentReference> referenceRepository,
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IReadRepository<Service> serviceReadRepository)
    : TransactionXmlReaderBase<UsageTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "BigData_867_03";
    private const string DetailUsageType = "DET";
    protected override string RequiredElementName => "Usage";

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var usageTransaction = transaction.Usage;

        var commodityId = valueMapper.GetCommodityId(transaction.Commodity);

        var usageTypeId = valueMapper.GetUsageTypeId(usageTransaction.PurposeCode ?? string.Empty);
        var isFinal = usageTransaction.FinalIndicator == "F";
        var originalUsageId = await GetOriginalUsageIdAsync(usageTransaction.OriginalTransactionNumber, ct);

        foreach (var usageMeter in usageTransaction.UsageMeters.Where(x => x.UsageType == DetailUsageType))
        {
            var periodStartDate = DateOnly.ParseExact(usageMeter.ServicePeriodBeginDate, DateFormat, CultureInfo.InvariantCulture);
            var periodEndDate = DateOnly.ParseExact(usageMeter.ServicePeriodEndDate, DateFormat, CultureInfo.InvariantCulture);

            var service = await GetServiceAsync(
                usageTransaction.UtilityAccountNumber, 
                usageTransaction.TranNr814, 
                isFinal, 
                periodStartDate, 
                periodEndDate, 
                ct);

            var meter = service.ServicePoint.GetMeter(usageMeter.MeterIdentifier);

            foreach (var quantity in usageMeter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var usage = new Usage
                    {
                        UsageTypeId = usageTypeId,
                        OriginalUsageId = originalUsageId,
                        IsFinal = isFinal,
                        StartRead = read.StartMeterRead ?? 0,
                        EndRead = read.EndMeterRead ?? 0,
                        UsageStatusId = Constants.UsageStatuses.New,
                        ServiceId = service.Id,
                        Identifier = item.ReferenceNumber,
                        ServicePointId = service.ServicePointId,
                        MeterId = meter?.Id,
                        PeriodStartDate = periodStartDate,
                        PeriodEndDate = periodEndDate,
                        Consumption = read.ReadConsumption,
                        CommodityId = commodityId,
                        UnitOfMeasureId = valueMapper.GetUnitOfMeasureId(read.ReadUOM),
                    };

                    var created = await usageRepository.AddAsync(usage, ct);

                    var reference = new MarketDocumentReference
                    {
                        MarketDocumentItemId = item.Id,
                        ReferenceTypeId = Constants.MarketDocumentReferenceTypes.Usage,
                        ReferenceId = created.Id,
                    };

                    await referenceRepository.AddAsync(reference, ct);
                }
            }
        }
    }

    private async Task<long?> GetOriginalUsageIdAsync(string? originalTransactionNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(originalTransactionNumber))
        {
            return null;
        }

        var originalUsage = await usageReadRepository.FirstOrDefaultAsync(
            new UsageByIdentifierSpec(originalTransactionNumber), ct)
            ?? throw new InvalidOperationException($"Original usage '{originalTransactionNumber}' was not found.");

        return originalUsage.Id;
    }

    private async Task<Service> GetServiceAsync(
        string servicePointIdentifier, 
        string? tranNr814, 
        bool isFinal,
        DateOnly periodStartDate, 
        DateOnly periodEndDate, 
        CancellationToken ct)
    {
        if (isFinal && !string.IsNullOrWhiteSpace(tranNr814))
        {
            var serviceStartItem = await serviceStartItemRepository.FirstOrDefaultAsync(
                new ServiceStartItemByIdentifierSpec(tranNr814), ct);

            if (serviceStartItem is not null)
            {
                return serviceStartItem.ServiceStart.Service;
            }
        }

        var services = await serviceReadRepository.ListAsync(
            new ServiceByServicePointAndPeriodSpec(servicePointIdentifier, periodStartDate, periodEndDate), ct);

        if (services.Count == 0)
        {
            throw new InvalidOperationException(
                $"No Service was found for ServicePoint '{servicePointIdentifier}' covering period {periodStartDate:yyyy-MM-dd} to {periodEndDate:yyyy-MM-dd}.");
        }

        if (services.Count > 1)
        {
            throw new InvalidOperationException(
                $"Multiple Services ({services.Count}) were found for ServicePoint '{servicePointIdentifier}' covering period {periodStartDate:yyyy-MM-dd} to {periodEndDate:yyyy-MM-dd}.");
        }

        return services.First();
    }
}
