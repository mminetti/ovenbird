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
        var customerClass = await customerClassRepository.FirstOrDefaultAsync(new CustomerClassByNameSpec(name ?? string.Empty), ct);

        return customerClass is not null
            ? Result.Success(customerClass)
            : Result.Invalid(new ValidationError(nameof(CustomerClass), $"Unmapped customer class '{name}'."));
    }

    public async Task<Result<LoadProfile>> GetLoadProfileByNameAsync(string? name, CancellationToken ct)
    {
        var loadProfile = await loadProfileRepository.FirstOrDefaultAsync(new LoadProfileByNameSpec(name ?? string.Empty), ct);

        return loadProfile is not null
            ? Result.Success(loadProfile)
            : Result.Invalid(new ValidationError(nameof(LoadProfile), $"Unmapped load profile '{name}'."));
    }

    public async Task<Result<ReadCycle>> GetReadCycleByNameAsync(string? name, CancellationToken ct)
    {
        var readCycle = await readCycleRepository.FirstOrDefaultAsync(new ReadCycleByNameSpec(name ?? string.Empty), ct);

        return readCycle is not null
            ? Result.Success(readCycle)
            : Result.Invalid(new ValidationError(nameof(ReadCycle), $"Unmapped read cycle '{name}'."));
    }

    public async Task<Result<County>> GetCountyByNameAsync(string? name, CancellationToken ct)
    {
        var county = await countyRepository.FirstOrDefaultAsync(new CountyByNameSpec(name ?? string.Empty), ct);

        return county is not null
            ? Result.Success(county)
            : Result.Invalid(new ValidationError(nameof(County), $"Unmapped county '{name}'."));
    }
}
