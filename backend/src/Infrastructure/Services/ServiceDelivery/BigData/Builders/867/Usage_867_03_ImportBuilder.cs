using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Core.Subscription;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

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

    public async Task<Result> CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transactionResult = DeserializeTransaction(item);
        if (!transactionResult.IsSuccess)
        {
            return AsResult(transactionResult);
        }

        var transaction = transactionResult.Value;
        var usageTransaction = transaction.Usage;

        var commodityIdResult = valueMapper.GetCommodityId(transaction.Commodity);
        if (!commodityIdResult.IsSuccess)
        {
            return AsResult(commodityIdResult);
        }

        var commodityId = commodityIdResult.Value;

        var usageTypeIdResult = valueMapper.GetUsageTypeId(usageTransaction.PurposeCode ?? string.Empty);
        if (!usageTypeIdResult.IsSuccess)
        {
            return AsResult(usageTypeIdResult);
        }

        var usageTypeId = usageTypeIdResult.Value;
        var isFinal = usageTransaction.FinalIndicator == "F";

        var originalUsageIdResult = await GetOriginalUsageIdAsync(usageTransaction.OriginalTransactionNumber, ct);
        if (!originalUsageIdResult.IsSuccess)
        {
            return AsResult(originalUsageIdResult);
        }

        var originalUsageId = originalUsageIdResult.Value;

        var pendingUsages = new List<Usage>();

        foreach (var usageMeter in usageTransaction.UsageMeters.Where(x => x.UsageType == DetailUsageType))
        {
            var periodStartDate = DateOnly.ParseExact(usageMeter.ServicePeriodBeginDate, DateFormat, CultureInfo.InvariantCulture);
            var periodEndDate = DateOnly.ParseExact(usageMeter.ServicePeriodEndDate, DateFormat, CultureInfo.InvariantCulture);

            var serviceResult = await GetServiceAsync(
                usageTransaction.UtilityAccountNumber,
                usageTransaction.TranNr814,
                isFinal,
                periodStartDate,
                periodEndDate,
                ct);

            if (!serviceResult.IsSuccess)
            {
                return AsResult(serviceResult);
            }

            var service = serviceResult.Value;
            var meter = service.ServicePoint.GetMeter(usageMeter.MeterIdentifier);

            foreach (var quantity in usageMeter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var unitOfMeasureIdResult = valueMapper.GetUnitOfMeasureId(read.ReadUOM);
                    if (!unitOfMeasureIdResult.IsSuccess)
                    {
                        return AsResult(unitOfMeasureIdResult);
                    }

                    pendingUsages.Add(new Usage
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
                        UnitOfMeasureId = unitOfMeasureIdResult.Value,
                    });
                }
            }
        }

        foreach (var usage in pendingUsages)
        {
            var created = await usageRepository.AddAsync(usage, ct);

            var reference = new MarketDocumentReference
            {
                MarketDocumentItemId = item.Id,
                ReferenceTypeId = Constants.MarketDocumentReferenceTypes.Usage,
                ReferenceId = created.Id,
            };

            await referenceRepository.AddAsync(reference, ct);
        }

        return Result.Success();
    }

    private async Task<Result<long?>> GetOriginalUsageIdAsync(string? originalTransactionNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(originalTransactionNumber))
        {
            return Result.Success<long?>(null);
        }

        var originalUsage = await usageReadRepository.FirstOrDefaultAsync(
            new UsageByIdentifierSpec(originalTransactionNumber), ct);

        return originalUsage is not null
            ? Result.Success<long?>(originalUsage.Id)
            : Result.Invalid(new ValidationError(nameof(originalTransactionNumber), $"Original usage '{originalTransactionNumber}' was not found."));
    }

    private async Task<Result<Service>> GetServiceAsync(
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
                return Result.Success(serviceStartItem.ServiceStart.Service);
            }
        }

        var services = await serviceReadRepository.ListAsync(
            new ServiceByServicePointAndPeriodSpec(servicePointIdentifier, periodStartDate, periodEndDate), ct);

        if (services.Count == 0)
        {
            return Result.Invalid(new ValidationError(
                nameof(servicePointIdentifier),
                $"No Service was found for ServicePoint '{servicePointIdentifier}' covering period {periodStartDate:yyyy-MM-dd} to {periodEndDate:yyyy-MM-dd}."));
        }

        if (services.Count > 1)
        {
            return Result.Invalid(new ValidationError(
                nameof(servicePointIdentifier),
                $"Multiple Services ({services.Count}) were found for ServicePoint '{servicePointIdentifier}' covering period {periodStartDate:yyyy-MM-dd} to {periodEndDate:yyyy-MM-dd}."));
        }

        return Result.Success(services.First());
    }
}
