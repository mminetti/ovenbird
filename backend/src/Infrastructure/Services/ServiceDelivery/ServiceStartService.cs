using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Interfaces.ServiceDelivery;

namespace Infrastructure.Services.ServiceDelivery;

public class ServiceStartService(IReadRepository<ServiceStartItem> serviceStartItemRepository) : IServiceStartService
{
    public async Task<Result<ServiceStart>> GetByIdentifierAsync(string? identifier, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Invalid(new ValidationError(nameof(ServiceStart), "ServiceStart identifier is required."));
        }

        var serviceStartItem = await serviceStartItemRepository.FirstOrDefaultAsync(
            new ServiceStartItemByIdentifierSpec(identifier), ct);

        return serviceStartItem is not null
            ? Result.Success(serviceStartItem.ServiceStart)
            : Result.Invalid(new ValidationError(nameof(ServiceStart), $"ServiceStart with identifier '{identifier}' was not found."));
    }
}
