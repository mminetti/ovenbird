using Core.ServiceDelivery;

namespace UseCases.Interfaces.ServiceDelivery;

public interface IServiceStartService
{
    Task<Result<ServiceStart>> GetByIdentifierAsync(string? identifier, CancellationToken ct);
}
