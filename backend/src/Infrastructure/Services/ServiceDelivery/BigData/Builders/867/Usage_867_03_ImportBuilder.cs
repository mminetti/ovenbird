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
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IReadRepository<Service> serviceReadRepository)
    : TransactionXmlReaderBase<UsageTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "BigData_867_03";
    private const string DetailUsageType = "DET";
    protected override string RequiredElementName => "Usage";

    public int EventTypeId => Constants.MarketDocumentEventTypes.Usage;

    public async Task<Result<IReadOnlyList<long>>> CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transactionResult = DeserializeTransaction(item);
        if (!transactionResult.IsSuccess)
        {
            return AsResult(transactionResult);
        }

        var transaction = transactionResult.Value;
        var usageTransaction = transaction.Usage;

        var commodityIdResult = valueMapper.GetCommodityId(transaction.Commodity);
        var usageTypeIdResult = valueMapper.GetUsageTypeId(usageTransaction.PurposeCode ?? string.Empty);
        var originalUsageIdResult = await GetOriginalUsageIdAsync(usageTransaction.OriginalTransactionNumber, ct);

        if (!commodityIdResult.IsSuccess || !usageTypeIdResult.IsSuccess || !originalUsageIdResult.IsSuccess)
        {
            return Result<IReadOnlyList<long>>.Invalid(
                commodityIdResult.ValidationErrors
                    .Concat(usageTypeIdResult.ValidationErrors)
                    .Concat(originalUsageIdResult.ValidationErrors)
                    .ToList());
        }

        var commodityId = commodityIdResult.Value;
        var usageTypeId = usageTypeIdResult.Value;
        var originalUsageId = originalUsageIdResult.Value;
        var isFinal = usageTransaction.FinalIndicator == "F";

        var pendingUsages = new List<Usage>();
        var errors = new List<ValidationError>();

        foreach (var usageMeter in usageTransaction.UsageMeters.Where(x => x.UsageType == DetailUsageType))
        {
            if (!DateOnly.TryParseExact(usageMeter.ServicePeriodBeginDate, DateFormat, out var periodStartDate) ||
                !DateOnly.TryParseExact(usageMeter.ServicePeriodEndDate, DateFormat, out var periodEndDate))
            {
                errors.Add(new ValidationError(
                    nameof(usageMeter.ServicePeriodBeginDate), $"Service Period is invalid for meter '{usageMeter.MeterIdentifier}'."));
                continue;
            }

            var serviceResult = await GetServiceAsync(
                usageTransaction.UtilityAccountNumber,
                usageTransaction.TranNr814,
                isFinal,
                periodStartDate,
                periodEndDate,
                ct);

            if (!serviceResult.IsSuccess)
            {
                errors.AddRange(serviceResult.ValidationErrors);
                continue;
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
                        errors.AddRange(unitOfMeasureIdResult.ValidationErrors);
                        continue;
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

        if (errors.Count > 0)
        {
            return Result<IReadOnlyList<long>>.Invalid(errors);
        }

        var createdIds = new List<long>();

        foreach (var usage in pendingUsages)
        {
            var created = await usageRepository.AddAsync(usage, ct);
            createdIds.Add(created.Id);
        }

        return Result.Success<IReadOnlyList<long>>(createdIds);
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
