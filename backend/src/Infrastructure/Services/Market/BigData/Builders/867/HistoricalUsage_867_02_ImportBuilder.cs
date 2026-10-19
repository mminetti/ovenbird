using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.Subscription;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public class HistoricalUsage_867_02_ImportBuilder(
    IBigDataValueMapper valueMapper,
    IRepository<HistoricalUsage> historicalUsageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<UsageTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "BigData_867_02";

    protected override string RequiredElementName => "Usage";

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var usageTransaction = transaction.Usage;

        var commodityId = valueMapper.GetCommodityId(transaction.Commodity);

        var service = await GetServiceAsync(usageTransaction.UtilityAccountNumber, ct);

        foreach (var usageMeter in usageTransaction.UsageMeters)
        {
            var meter = await GetMeter(service.ServicePoint, usageMeter.MeterIdentifier, ct);

            var periodStartDate = DateOnly.ParseExact(usageMeter.ServicePeriodBeginDate, DateFormat, CultureInfo.InvariantCulture);
            var periodEndDate = DateOnly.ParseExact(usageMeter.ServicePeriodEndDate, DateFormat, CultureInfo.InvariantCulture);

            foreach (var quantity in usageMeter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var usage = new HistoricalUsage
                    {
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

                    var created = await historicalUsageRepository.AddAsync(usage, ct);

                    var reference = new MarketDocumentReference
                    {
                        MarketDocumentItemId = item.Id,
                        ReferenceTypeId = Constants.MarketDocumentReferenceTypes.HistoricalUsage,
                        ReferenceId = created.Id,
                    };

                    await referenceRepository.AddAsync(reference, ct);
                }
            }
        }
    }

    private Task<Service> GetServiceAsync(string identifier, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    private Task<Meter?> GetMeter(ServicePoint servicePoint, string? meterIdentifier, CancellationToken ct)
    {
        //return servicePoint.GetMeter(meterIdentifier);
        throw new NotImplementedException();
    }
}
