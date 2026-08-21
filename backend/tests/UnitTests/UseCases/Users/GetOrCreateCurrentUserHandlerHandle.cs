using Core.Security;
using Core.Security.Specifications;
using Microsoft.Extensions.Caching.Hybrid;
using UnitTests.Helpers;
using UseCases.Security.Users;

namespace UnitTests.UseCases.Users;

public class GetOrCreateCurrentUserHandlerHandle
{
    private readonly IReadRepository<User> _readRepository = Substitute.For<IReadRepository<User>>();
    private readonly IRepository<User> _writeRepository = Substitute.For<IRepository<User>>();
    private readonly HybridCache _cache = new PassThroughHybridCache();
    private readonly CurrentUserService _service;

    public GetOrCreateCurrentUserHandlerHandle()
    {
        _service = new CurrentUserService(_readRepository, _writeRepository, _cache);
    }

    [Fact]
    public async Task WhenUserExists_ReturnsExistingUserWithPermissions()
    {
        var module = new Module { Id = 1, Name = "Security" };
        var permission1 = new Permission { Id = 1, Name = "Users:Read", Module = module };
        var permission2 = new Permission { Id = 2, Name = "Modules:Read", Module = module };
        var role = new Role { Id = 10, Name = "Admin", Permissions = [permission1, permission2] };
        var user = new User { Id = 5, ExternalIdentifier = "oid-123", Name = "Alice", Email = "alice@test.com", IsActive = true, Roles = [role] };

        _readRepository.FirstOrDefaultAsync(
            Arg.Any<UserWithRolesAndPermissionsByExternalIdentifierSpec>(),
            Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _service.GetOrCreateAsync("oid-123", "Alice", "alice@test.com", CancellationToken.None);

        result.UserId.ShouldBe(5);
        result.ExternalIdentifier.ShouldBe("oid-123");
        result.Name.ShouldBe("Alice");
        result.Email.ShouldBe("alice@test.com");
        result.Permissions.ShouldContain(p => p.Name == "Users:Read");
        result.Permissions.ShouldContain(p => p.Name == "Modules:Read");
        result.Permissions.Count.ShouldBe(2);
    }

    [Fact]
    public async Task WhenUserDoesNotExist_CreatesNewUser_WithIsActiveTrue_AndNoRoles()
    {
        _readRepository.FirstOrDefaultAsync(
            Arg.Any<UserWithRolesAndPermissionsByExternalIdentifierSpec>(),
            Arg.Any<CancellationToken>())
            .Returns((User?)null);

        User? captured = null;
        _writeRepository.AddAsync(Arg.Do<User>(u => captured = u), Arg.Any<CancellationToken>())
            .Returns(c =>
            {
                var u = c.Arg<User>();
                u.Id = 99;
                return u;
            });

        await _service.GetOrCreateAsync("oid-new", "Bob", "bob@test.com", CancellationToken.None);

        captured.ShouldNotBeNull();
        captured!.IsActive.ShouldBeTrue();
        captured.ExternalIdentifier.ShouldBe("oid-new");
        captured.Name.ShouldBe("Bob");
        captured.Email.ShouldBe("bob@test.com");
        captured.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task WhenUserDoesNotExist_ReturnsEmptyPermissions()
    {
        _readRepository.FirstOrDefaultAsync(
            Arg.Any<UserWithRolesAndPermissionsByExternalIdentifierSpec>(),
            Arg.Any<CancellationToken>())
            .Returns((User?)null);

        _writeRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(c =>
            {
                var u = c.Arg<User>();
                u.Id = 1;
                return u;
            });

        var result = await _service.GetOrCreateAsync("oid-new", "Bob", "bob@test.com", CancellationToken.None);

        result.Permissions.ShouldBeEmpty();
    }
}
