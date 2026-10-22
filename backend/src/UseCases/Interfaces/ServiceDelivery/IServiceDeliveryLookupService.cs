using Core.ServiceDelivery;

namespace UseCases.Interfaces.ServiceDelivery;

public interface IServiceDeliveryLookupService
{
    Task<Result<CustomerClass>> GetCustomerClassByNameAsync(string? name, CancellationToken ct);
    Task<Result<LoadProfile>> GetLoadProfileByNameAsync(string? name, CancellationToken ct);
    Task<Result<ReadCycle>> GetReadCycleByNameAsync(string? name, CancellationToken ct);
    Task<Result<County>> GetCountyByNameAsync(string? name, CancellationToken ct);
}
