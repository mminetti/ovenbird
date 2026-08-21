using Core.Security.Events;
using Microsoft.Extensions.Caching.Hybrid;

namespace UseCases.Security.Roles.Handlers;

public class RoleCacheInvalidationHandler(HybridCache cache)
{
    public async Task Handle(RoleDeletedEvent domainEvent, CancellationToken ct)
    {
        await cache.RemoveByTagAsync(Constants.CacheKeys.SecurityTag, ct);
    }

    public async Task Handle(RolePermissionsUpdatedEvent domainEvent, CancellationToken ct)
    {
        await cache.RemoveByTagAsync(Constants.CacheKeys.SecurityTag, ct);
    }
}
