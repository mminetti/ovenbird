using Core.ServiceDelivery;

namespace Infrastructure.Services.Market.BigData.Builders;

public interface IBigDataLookupService
{
    int GetCommodityId(string code);
    int GetUnitOfMeasureId(string code);
    int GetUsageTypeId(string purposeCode);
    Task<ServicePoint> GetServicePointAsync(string identifier, CancellationToken ct);
}
