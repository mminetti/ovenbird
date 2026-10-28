using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
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
    public async Task<Result> ExecuteAsync(ServiceStartScheduledEvent domainEvent, CancellationToken ct)
    {
        var commodityIdResult = valueMapper.GetCommodityId(domainEvent.CommodityCode);
        if (!commodityIdResult.IsSuccess)
        {
            return AsResult(commodityIdResult);
        }

        var commodityId = commodityIdResult.Value;

        if (string.IsNullOrWhiteSpace(domainEvent.PremiseAddressLine1))
        {
            return Result.Invalid(new ValidationError(nameof(domainEvent.PremiseAddressLine1), "Premise Address is required."));
        }

        if (string.IsNullOrWhiteSpace(domainEvent.PremiseCity))
        {
            return Result.Invalid(new ValidationError(nameof(domainEvent.PremiseCity), "Premise City is required."));
        }

        if (string.IsNullOrWhiteSpace(domainEvent.PremiseState))
        {
            return Result.Invalid(new ValidationError(nameof(domainEvent.PremiseState), "Premise State is required."));
        }

        var (zip, zip4) = SplitZip(domainEvent.PremisePostalCode);

        var countyResult = await lookupService.GetCountyByNameAsync(domainEvent.CountyName, ct);
        if (!countyResult.IsSuccess)
        {
            return AsResult(countyResult);
        }

        var county = countyResult.Value;

        var customerClassResult = await lookupService.GetCustomerClassByNameAsync(domainEvent.CustomerClassCode, ct);
        if (!customerClassResult.IsSuccess)
        {
            return AsResult(customerClassResult);
        }

        var customerClass = customerClassResult.Value;

        var loadProfileResult = await lookupService.GetLoadProfileByNameAsync(domainEvent.LoadProfileCode, ct);
        if (!loadProfileResult.IsSuccess)
        {
            return AsResult(loadProfileResult);
        }

        var loadProfile = loadProfileResult.Value;

        var readCycleResult = await lookupService.GetReadCycleByNameAsync(domainEvent.ReadCycleCode, ct);
        if (!readCycleResult.IsSuccess)
        {
            return AsResult(readCycleResult);
        }

        var readCycle = readCycleResult.Value;

        if (string.IsNullOrWhiteSpace(domainEvent.MeterNumber))
        {
            return Result.Invalid(new ValidationError(nameof(domainEvent.MeterNumber), "Service Meter Number is required."));
        }

        var servicePoint = await servicePointRepository.FirstOrDefaultAsync(
            new ServicePointByIdentifierAndCommoditySpec(domainEvent.ServicePointIdentifier, commodityId), ct);

        if (servicePoint is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(ServicePoint), $"ServicePoint '{domainEvent.ServicePointIdentifier}' was not found."));
        }

        servicePoint.CustomerClassId = customerClass.Id;
        servicePoint.LoadProfileId = loadProfile.Id;
        servicePoint.ReadCycleId = readCycle.Id;
        servicePoint.IsAmsMeter = valueMapper.IsAmsMeter(domainEvent.AdvanceMeterInfoCode);

        servicePoint.AddMeter(domainEvent.MeterNumber);

        var premise = servicePoint.Premise;
        premise.AddressLine1 = domainEvent.PremiseAddressLine1;
        premise.City = domainEvent.PremiseCity;
        premise.State = domainEvent.PremiseState;
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
