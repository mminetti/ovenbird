using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;
using UseCases.Interfaces.ServiceDelivery;

namespace Infrastructure.Services.ServiceDelivery;

public class UpdateServicePointAction(
    IRepository<ServicePoint> servicePointRepository,
    IServiceDeliveryLookupService lookupService,
    IBigDataValueMapper valueMapper)
    : IUpdateServicePointAction
{
    public async Task<Result> ExecuteAsync(UpdateServicePointRequest request, CancellationToken ct)
    {
        var commodityIdResult = valueMapper.GetCommodityId(request.CommodityCode);
        if (!commodityIdResult.IsSuccess)
        {
            return AsResult(commodityIdResult);
        }

        var commodityId = commodityIdResult.Value;

        if (string.IsNullOrWhiteSpace(request.PremiseAddressLine1))
        {
            return Result.Invalid(new ValidationError(nameof(request.PremiseAddressLine1), "Premise Address is required."));
        }

        if (string.IsNullOrWhiteSpace(request.PremiseCity))
        {
            return Result.Invalid(new ValidationError(nameof(request.PremiseCity), "Premise City is required."));
        }

        if (string.IsNullOrWhiteSpace(request.PremiseState))
        {
            return Result.Invalid(new ValidationError(nameof(request.PremiseState), "Premise State is required."));
        }

        var (zip, zip4) = SplitZip(request.PremisePostalCode);

        var countyResult = await lookupService.GetCountyByNameAsync(request.CountyName, ct);
        if (!countyResult.IsSuccess)
        {
            return AsResult(countyResult);
        }

        var county = countyResult.Value;

        var customerClassResult = await lookupService.GetCustomerClassByNameAsync(request.CustomerClassCode, ct);
        if (!customerClassResult.IsSuccess)
        {
            return AsResult(customerClassResult);
        }

        var customerClass = customerClassResult.Value;

        var loadProfileResult = await lookupService.GetLoadProfileByNameAsync(request.LoadProfileCode, ct);
        if (!loadProfileResult.IsSuccess)
        {
            return AsResult(loadProfileResult);
        }

        var loadProfile = loadProfileResult.Value;

        var readCycleResult = await lookupService.GetReadCycleByNameAsync(request.ReadCycleCode, ct);
        if (!readCycleResult.IsSuccess)
        {
            return AsResult(readCycleResult);
        }

        var readCycle = readCycleResult.Value;

        if (string.IsNullOrWhiteSpace(request.MeterNumber))
        {
            return Result.Invalid(new ValidationError(nameof(request.MeterNumber), "Service Meter Number is required."));
        }

        var servicePoint = await servicePointRepository.FirstOrDefaultAsync(
            new ServicePointByIdentifierAndCommoditySpec(request.ServicePointIdentifier, commodityId), ct);

        if (servicePoint is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(ServicePoint), $"ServicePoint '{request.ServicePointIdentifier}' was not found."));
        }

        servicePoint.CustomerClassId = customerClass.Id;
        servicePoint.LoadProfileId = loadProfile.Id;
        servicePoint.ReadCycleId = readCycle.Id;
        servicePoint.IsAmsMeter = valueMapper.IsAmsMeter(request.AdvanceMeterInfoCode);

        servicePoint.AddMeter(request.MeterNumber);

        var premise = servicePoint.Premise;
        premise.AddressLine1 = request.PremiseAddressLine1;
        premise.City = request.PremiseCity;
        premise.State = request.PremiseState;
        premise.Zip = zip;
        premise.Zip4 = zip4;
        premise.CountyId = county.Id;

        await servicePointRepository.UpdateAsync(servicePoint, ct);

        return Result.Success();
    }

    private static Result AsResult<T>(Result<T> result) => Result.Invalid(result.ValidationErrors.ToList());

    private static (string Zip, string? Zip4) SplitZip(string? postalCode)
    {
        var value = postalCode ?? string.Empty;

        return value.Length == 9
            ? (value[..5], value[5..])
            : (value, null);
    }
}
