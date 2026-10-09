namespace Infrastructure.Services.Market.BigData.EntityBuilders;

public interface IBigDataLookupService
{
    int GetCommodityId(string code);
    int GetUnitOfMeasureId(string code);
    Task<long> GetServicePointId(string? identifier, CancellationToken ct);
    Task<long?> GetMeterId(string? identifier, CancellationToken ct);
}
