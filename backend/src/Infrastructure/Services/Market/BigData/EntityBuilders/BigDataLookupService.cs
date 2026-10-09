using Core.Service;
using Core.Service.Specifications;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.EntityBuilders;

public class BigDataLookupService(
    IReadRepository<ServicePoint> servicePointReadRepository)
    : IBigDataLookupService
{
    private static readonly Dictionary<string, int> _commodityIdsByCode = new()
    {
        ["E"] = CoreConstants.Commodities.Electricity,
    };

    private static readonly Dictionary<string, int> _unitOfMeasureIdsByCode = new()
    {
        ["KH"] = CoreConstants.UnitOfMeasures.KilowattHour,
    };

    public int GetCommodityId(string code) =>
        _commodityIdsByCode.TryGetValue(code, out var id)
            ? id
            : throw new InvalidOperationException($"Unmapped commodity code '{code}'.");

    public int GetUnitOfMeasureId(string code) =>
        _unitOfMeasureIdsByCode.TryGetValue(code, out var id)
            ? id
            : throw new InvalidOperationException($"Unmapped unit of measure code '{code}'.");

    public async Task<ServicePoint> GetServicePointAsync(string identifier, CancellationToken ct)
    {
        return await servicePointReadRepository.FirstOrDefaultAsync(
            new ServicePointWithMetersByIdentifierSpec(identifier), ct)
            ?? throw new InvalidOperationException($"Service Point '{identifier}' was not found.");
    }
}
