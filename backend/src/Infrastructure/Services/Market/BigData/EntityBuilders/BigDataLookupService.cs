using Core.Service;
using Core.Service.Specifications;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.EntityBuilders;

public class BigDataLookupService(
    IReadRepository<ServicePoint> servicePointReadRepository,
    IReadRepository<Meter> meterReadRepository)
    : IBigDataLookupService
{
    private static readonly Dictionary<string, int> CommodityIdsByCode = new()
    {
        ["E"] = CoreConstants.Commodities.Electricity,
    };

    private static readonly Dictionary<string, int> UnitOfMeasureIdsByCode = new()
    {
        ["KH"] = CoreConstants.UnitOfMeasures.KilowattHour,
    };

    public int GetCommodityId(string code) =>
        CommodityIdsByCode.TryGetValue(code, out var id)
            ? id
            : throw new InvalidOperationException($"Unmapped commodity code '{code}'.");

    public int GetUnitOfMeasureId(string code) =>
        UnitOfMeasureIdsByCode.TryGetValue(code, out var id)
            ? id
            : throw new InvalidOperationException($"Unmapped unit of measure code '{code}'.");

    public async Task<long> GetServicePointId(string? identifier, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new InvalidOperationException("A ServicePoint identifier is required but was not provided.");
        }

        var servicePoint = await servicePointReadRepository.FirstOrDefaultAsync(new ServicePointByIdentifierSpec(identifier), ct)
            ?? throw new InvalidOperationException($"ServicePoint '{identifier}' was not found.");

        return servicePoint.Id;
    }

    public async Task<long?> GetMeterId(string? identifier, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return null;
        }

        var meter = await meterReadRepository.FirstOrDefaultAsync(new MeterByIdentifierSpec(identifier), ct)
            ?? throw new InvalidOperationException($"Meter '{identifier}' was not found.");

        return meter.Id;
    }
}
