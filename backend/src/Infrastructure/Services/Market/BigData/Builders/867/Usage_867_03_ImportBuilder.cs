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
    IRepository<MarketDocumentReference> referenceRepository)
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

        var service = await GetServiceAsync(usageTransaction.UtilityAccountNumber, ct);

        var usageTypeId = valueMapper.GetUsageTypeId(usageTransaction.PurposeCode ?? string.Empty);
        var isFinal = usageTransaction.FinalIndicator == "F";
        var originalUsageId = await GetOriginalUsageIdAsync(usageTransaction.OriginalTransactionNumber, ct);

        foreach (var usageMeter in usageTransaction.UsageMeters.Where(x => x.UsageType == DetailUsageType))
        {
            var meter = service.ServicePoint.GetMeter(usageMeter.MeterIdentifier);

            var periodStartDate = DateOnly.ParseExact(usageMeter.ServicePeriodBeginDate, DateFormat, CultureInfo.InvariantCulture);
            var periodEndDate = DateOnly.ParseExact(usageMeter.ServicePeriodEndDate, DateFormat, CultureInfo.InvariantCulture);

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

    private Task<Service> GetServiceAsync(string identifier, CancellationToken ct)
    {
        // if this is a final read, check if we can find a end service item with identifier = TranNr814
        // if we do, use it to get the service
        // if not, get all services for that service point identifier, with start and end date within the read period
        // if 0 or more than one, return error
        // if only one, return the service, including service point and meters

        throw new NotImplementedException();
    }
}
