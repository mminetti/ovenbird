using Core.Service;

namespace Infrastructure.Services.Market.BigData.EntityBuilders;

public interface IBigDataLookupService
{
    int GetCommodityId(string code);
    int GetUnitOfMeasureId(string code);
    Task<ServicePoint> GetServicePointAsync(string identifier, CancellationToken ct);
}
