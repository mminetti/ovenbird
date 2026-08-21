using Ardalis.SharedKernel;
using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Roles.SetPermissions;

namespace UnitTests.UseCases.Security.Roles;

public class SetRolePermissionsHandlerHandle
{
    private readonly IRepository<Role> _roleRepository = Substitute.For<IRepository<Role>>();
    private readonly IReadRepository<Permission> _permissionRepository = Substitute.For<IReadRepository<Permission>>();
    private readonly SetRolePermissionsHandler _handler;

    public SetRolePermissionsHandlerHandle()
    {
        _handler = new SetRolePermissionsHandler(_roleRepository, _permissionRepository);
    }

    [Fact]
    public async Task ReturnsSuccessAndAssignsNewPermissions()
    {
        var role = new Role { Id = 1, Name = "Admin", Permissions = [] };
        var permissions = new List<Permission>
        {
            new() { Id = 10, Name = "users.read" },
            new() { Id = 20, Name = "users.write" }
        };

        _roleRepository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);
        _permissionRepository.ListAsync(Arg.Any<PermissionsByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns(permissions);

        var result = await _handler.Handle(
            new SetRolePermissionsCommand(1, [10, 20]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        role.Permissions.Count.ShouldBe(2);
        await _roleRepository.Received(1).UpdateAsync(role, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenRoleDoesNotExist()
    {
        _roleRepository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Role?)null);

        var result = await _handler.Handle(
            new SetRolePermissionsCommand(999, [1]),
            CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _roleRepository.DidNotReceive().UpdateAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ClearsAllPermissionsWhenListIsEmpty()
    {
        var existing = new Permission { Id = 10, Name = "users.read" };
        var role = new Role { Id = 1, Name = "Admin", Permissions = [existing] };

        _roleRepository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);

        var result = await _handler.Handle(
            new SetRolePermissionsCommand(1, []),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        role.Permissions.ShouldBeEmpty();
        await _roleRepository.Received(1).UpdateAsync(role, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task KeepsAlreadyAssignedKeepsRemovesAndAddsCorrectly()
    {
        var keep = new Permission { Id = 10, Name = "users.read" };
        var remove = new Permission { Id = 20, Name = "users.write" };
        var role = new Role { Id = 1, Name = "Admin", Permissions = [keep, remove] };
        var add = new Permission { Id = 30, Name = "users.delete" };

        _roleRepository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);
        _permissionRepository.ListAsync(Arg.Any<PermissionsByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns(new List<Permission> { keep, add });

        var result = await _handler.Handle(
            new SetRolePermissionsCommand(1, [10, 30]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        role.Permissions.Select(p => p.Id).ShouldBe([10, 30], ignoreOrder: true);
    }
}
