using Core.Security;
using Core.Security.Specifications;
using Microsoft.Extensions.Caching.Hybrid;
using UseCases.Security.Users.GetOrCreate;

namespace UseCases.Security.Users;

public class CurrentUserService(
    IReadRepository<User> readRepository,
    IRepository<User> writeRepository,
    HybridCache cache)
{
    private static readonly HybridCacheEntryOptions _options = new()
    {
        Expiration = TimeSpan.FromMinutes(60)
    };

    public async Task<CurrentUserInfo> GetOrCreateAsync(string externalIdentifier, string name, string email, CancellationToken ct)
    {
        return await cache.GetOrCreateAsync(
            $"{Constants.CacheKeys.CurrentUserPrefix}{externalIdentifier}",
            async token =>
            {
                var existing = await readRepository.FirstOrDefaultAsync(
                    new UserWithRolesAndPermissionsByExternalIdentifierSpec(externalIdentifier), ct);

                if (existing is not null)
                {
                    return MapToCurrentUserInfo(existing);
                }

                var newUser = new User
                {
                    ExternalIdentifier = externalIdentifier,
                    Name = name,
                    Email = email,
                    IsActive = true
                };

                var created = await writeRepository.AddAsync(newUser, ct);

                return new CurrentUserInfo(created.Id, created.ExternalIdentifier, created.Name, created.Email, created.IsActive);
            },
            _options,
            [Constants.CacheKeys.SecurityTag],
            ct);
    }

    public async Task<CurrentUserInfo?> GetAsync(string externalIdentifier, CancellationToken ct)
    {
        return await cache.GetOrCreateAsync(
            $"{Constants.CacheKeys.CurrentUserPrefix}{externalIdentifier}",
            async token =>
            {
                var user = await readRepository.FirstOrDefaultAsync(
                    new UserWithRolesAndPermissionsByExternalIdentifierSpec(externalIdentifier), ct);

                return user is null ? null : MapToCurrentUserInfo(user);
            },
            _options,
            [Constants.CacheKeys.SecurityTag],
            ct);
    }

    private static CurrentUserInfo MapToCurrentUserInfo(User user)
    {
        var permissions = user.Roles
            .SelectMany(r => r.Permissions)
            .DistinctBy(p => p.Id)
            .Select(p => new UserPermissionDto(p.Name, p.Module.Name))
            .ToList();

        return new CurrentUserInfo(user.Id, user.ExternalIdentifier, user.Name, user.Email, user.IsActive) { Permissions = permissions };
    }
}
