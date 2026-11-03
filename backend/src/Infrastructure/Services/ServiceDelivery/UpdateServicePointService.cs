using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;
using UseCases.Interfaces.ServiceDelivery;

namespace Infrastructure.Services.ServiceDelivery;

public class UpdateServicePointService(
    IRepository<ServicePoint> servicePointRepository,
    IServiceDeliveryLookupService lookupService,
    IBigDataValueMapper valueMapper)
    : IUpdateServicePointService
{
    public async Task<Result> ExecuteAsync(UpdateServicePointRequest request, CancellationToken ct)
    {
        var commodityIdResult = valueMapper.GetCommodityId(request.CommodityCode);
        var premiseValidationResult = ValidatePremiseAddress(request);
        var meterNumberValidationResult = ValidateMeterNumber(request.MeterNumber);
        var countyResult = await lookupService.GetCountyByNameAsync(request.CountyName, ct);
        var customerClassResult = await lookupService.GetCustomerClassByNameAsync(request.CustomerClassCode, ct);
        var loadProfileResult = await lookupService.GetLoadProfileByNameAsync(request.LoadProfileCode, ct);
        var readCycleResult = await lookupService.GetReadCycleByNameAsync(request.ReadCycleCode, ct);

        if (!commodityIdResult.IsSuccess || !premiseValidationResult.IsSuccess || !meterNumberValidationResult.IsSuccess ||
            !countyResult.IsSuccess || !customerClassResult.IsSuccess || !loadProfileResult.IsSuccess || !readCycleResult.IsSuccess)
        {
            return Result.Invalid(commodityIdResult.ValidationErrors
                .Concat(premiseValidationResult.ValidationErrors)
                .Concat(meterNumberValidationResult.ValidationErrors)
                .Concat(countyResult.ValidationErrors)
                .Concat(customerClassResult.ValidationErrors)
                .Concat(loadProfileResult.ValidationErrors)
                .Concat(readCycleResult.ValidationErrors)
                .ToList());
        }

        var servicePoint = await servicePointRepository.FirstOrDefaultAsync(
            new ServicePointByIdentifierAndCommoditySpec(request.ServicePointIdentifier, commodityIdResult.Value), ct);

        if (servicePoint is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(ServicePoint), $"ServicePoint '{request.ServicePointIdentifier}' was not found."));
        }

        servicePoint.CustomerClassId = customerClassResult.Value.Id;
        servicePoint.LoadProfileId = loadProfileResult.Value.Id;
        servicePoint.ReadCycleId = readCycleResult.Value.Id;
        servicePoint.IsAmsMeter = valueMapper.IsAmsMeter(request.AmsMeterCode);

        servicePoint.AddMeter(request.MeterNumber!);

        var (zip, zip4) = SplitZip(request.PremiseZipCode);

        var premise = servicePoint.Premise;
        premise.AddressLine1 = request.PremiseAddressLine1!;
        premise.City = request.PremiseCity!;
        premise.State = request.PremiseState!;
        premise.Zip = zip;
        premise.Zip4 = zip4;
        premise.CountyId = countyResult.Value.Id;

        await servicePointRepository.UpdateAsync(servicePoint, ct);

        return Result.Success();
    }

    private static Result ValidatePremiseAddress(UpdateServicePointRequest request)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(request.PremiseAddressLine1))
        {
            errors.Add(new ValidationError(nameof(request.PremiseAddressLine1), "Premise Address is required."));
        }

        if (string.IsNullOrWhiteSpace(request.PremiseCity))
        {
            errors.Add(new ValidationError(nameof(request.PremiseCity), "Premise City is required."));
        }

        if (string.IsNullOrWhiteSpace(request.PremiseState))
        {
            errors.Add(new ValidationError(nameof(request.PremiseState), "Premise State is required."));
        }

        return errors.Count == 0 ? Result.Success() : Result.Invalid(errors);
    }

    private static Result ValidateMeterNumber(string? meterNumber) =>
        string.IsNullOrWhiteSpace(meterNumber)
            ? Result.Invalid(new ValidationError(nameof(UpdateServicePointRequest.MeterNumber), "Service Meter Number is required."))
            : Result.Success();

    private static (string Zip, string? Zip4) SplitZip(string? postalCode)
    {
        var value = postalCode ?? string.Empty;

        return value.Length == 9
            ? (value[..5], value[5..])
            : (value, null);
    }
}
