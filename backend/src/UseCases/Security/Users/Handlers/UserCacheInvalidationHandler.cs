using Core.Security.Events;
using Microsoft.Extensions.Caching.Hybrid;

namespace UseCases.Security.Users.Handlers;

public class UserCacheInvalidationHandler(HybridCache cache)
{
    public async Task Handle(UserDeletedEvent domainEvent, CancellationToken ct)
    {
        await cache.RemoveAsync($"{Constants.CacheKeys.CurrentUserPrefix}{domainEvent.User.ExternalIdentifier}", ct);
    }

    public async Task Handle(UserUpdatedEvent domainEvent, CancellationToken ct)
    {
        await cache.RemoveAsync($"{Constants.CacheKeys.CurrentUserPrefix}{domainEvent.User.ExternalIdentifier}", ct);
    }

    public async Task Handle(UserRolesUpdatedEvent domainEvent, CancellationToken ct)
    {
        await cache.RemoveAsync($"{Constants.CacheKeys.CurrentUserPrefix}{domainEvent.User.ExternalIdentifier}", ct);
    }
}
