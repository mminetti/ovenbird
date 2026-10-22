using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Interfaces.ServiceDelivery;

namespace Infrastructure.Services.ServiceDelivery;

public class ServiceDeliveryLookupService(
    IReadRepository<CustomerClass> customerClassRepository,
    IReadRepository<LoadProfile> loadProfileRepository,
    IReadRepository<ReadCycle> readCycleRepository,
    IReadRepository<County> countyRepository)
    : IServiceDeliveryLookupService
{
    public async Task<Result<CustomerClass>> GetCustomerClassByNameAsync(string? name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Invalid(new ValidationError(nameof(name), "Customer class name is required."));
        }

        var customerClass = await customerClassRepository.FirstOrDefaultAsync(new CustomerClassByNameSpec(name), ct);

        return customerClass is not null
            ? Result.Success(customerClass)
            : Result.Invalid(new ValidationError(nameof(CustomerClass), $"Unmapped customer class '{name}'."));
    }

    public async Task<Result<LoadProfile>> GetLoadProfileByNameAsync(string? name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Invalid(new ValidationError(nameof(name), "Load profile name is required."));
        }

        var loadProfile = await loadProfileRepository.FirstOrDefaultAsync(new LoadProfileByNameSpec(name), ct);

        return loadProfile is not null
            ? Result.Success(loadProfile)
            : Result.Invalid(new ValidationError(nameof(LoadProfile), $"Unmapped load profile '{name}'."));
    }

    public async Task<Result<ReadCycle>> GetReadCycleByNameAsync(string? name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Invalid(new ValidationError(nameof(name), "Read cycle name is required."));
        }

        var readCycle = await readCycleRepository.FirstOrDefaultAsync(new ReadCycleByNameSpec(name), ct);

        return readCycle is not null
            ? Result.Success(readCycle)
            : Result.Invalid(new ValidationError(nameof(ReadCycle), $"Unmapped read cycle '{name}'."));
    }

    public async Task<Result<County>> GetCountyByNameAsync(string? name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Invalid(new ValidationError(nameof(name), "County name is required."));
        }

        var county = await countyRepository.FirstOrDefaultAsync(new CountyByNameSpec(name), ct);

        return county is not null
            ? Result.Success(county)
            : Result.Invalid(new ValidationError(nameof(County), $"Unmapped county '{name}'."));
    }
}
