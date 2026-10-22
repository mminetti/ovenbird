using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Core.Subscription;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class HistoricalUsage_867_02_ImportBuilder(
    IBigDataValueMapper valueMapper,
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<HistoricalUsage> historicalUsageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<UsageTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "BigData_867_02";

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

        var serviceResult = await GetServiceAsync(usageTransaction.TranNr814, ct);
        if (!serviceResult.IsSuccess)
        {
            return AsResult(serviceResult);
        }

        var service = serviceResult.Value;

        var pendingUsages = new List<HistoricalUsage>();

        foreach (var usageMeter in usageTransaction.UsageMeters)
        {
            var meter = service.ServicePoint.GetMeter(usageMeter.MeterIdentifier);

            var periodStartDate = DateOnly.ParseExact(usageMeter.ServicePeriodBeginDate, DateFormat, CultureInfo.InvariantCulture);
            var periodEndDate = DateOnly.ParseExact(usageMeter.ServicePeriodEndDate, DateFormat, CultureInfo.InvariantCulture);

            foreach (var quantity in usageMeter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var unitOfMeasureIdResult = valueMapper.GetUnitOfMeasureId(read.ReadUOM);
                    if (!unitOfMeasureIdResult.IsSuccess)
                    {
                        return AsResult(unitOfMeasureIdResult);
                    }

                    pendingUsages.Add(new HistoricalUsage
                    {
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
            var created = await historicalUsageRepository.AddAsync(usage, ct);

            var reference = new MarketDocumentReference
            {
                MarketDocumentItemId = item.Id,
                ReferenceTypeId = Constants.MarketDocumentReferenceTypes.HistoricalUsage,
                ReferenceId = created.Id,
            };

            await referenceRepository.AddAsync(reference, ct);
        }

        return Result.Success();
    }

    private async Task<Result<Service>> GetServiceAsync(string? identifier, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Invalid(new ValidationError(nameof(identifier), "Transaction has no ProcessTrackingNumber."));
        }

        var serviceStartItem = await serviceStartItemRepository.FirstOrDefaultAsync(
            new ServiceStartItemByIdentifierSpec(identifier), ct);

        return serviceStartItem is not null
            ? Result.Success(serviceStartItem.ServiceStart.Service)
            : Result.Invalid(new ValidationError(nameof(identifier), $"ServiceStartItem with Identifier '{identifier}' was not found."));
    }
}
