using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Roles.Update;

namespace UnitTests.UseCases.Roles;

public class UpdateRoleHandlerHandle
{
    private readonly IRepository<Role> _repository = Substitute.For<IRepository<Role>>();
    private readonly IRepository<Permission> _permissionRepository = Substitute.For<IRepository<Permission>>();
    private readonly UpdateRoleHandler _handler;

    public UpdateRoleHandlerHandle()
    {
        _handler = new UpdateRoleHandler(_repository, _permissionRepository);
    }

    [Fact]
    public async Task ReturnsSuccessWhenRoleExists()
    {
        var role = new Role { Id = 1, Name = "Admin" };

        _repository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);

        var result = await _handler.Handle(new UpdateRoleCommand(1, "SuperAdmin", null), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        role.Name.ShouldBe("SuperAdmin");
        await _repository.Received(1).UpdateAsync(role, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenRoleDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Role?)null);

        var result = await _handler.Handle(new UpdateRoleCommand(999, "Name", null), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddsPermissionWhenOperationIsAdd()
    {
        var role = new Role { Id = 1, Name = "Admin", Permissions = [] };
        var permission = new Permission { Id = 10, Name = "users.read" };

        _repository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);
        _permissionRepository.ListAsync(Arg.Any<PermissionsByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([permission]);

        var result = await _handler.Handle(
            new UpdateRoleCommand(1, "Admin", [new RolePermissionOperationDto(10, "add")]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        role.Permissions.Select(p => p.Id).ShouldBe([10]);
    }

    [Fact]
    public async Task RemovesPermissionWhenOperationIsRemove()
    {
        var permission = new Permission { Id = 10, Name = "users.read" };
        var role = new Role { Id = 1, Name = "Admin", Permissions = [permission] };

        _repository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);

        var result = await _handler.Handle(
            new UpdateRoleCommand(1, "Admin", [new RolePermissionOperationDto(10, "remove")]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        role.Permissions.ShouldBeEmpty();
    }

    [Fact]
    public async Task LeavesOtherPermissionsUntouchedWhenAddingOrRemoving()
    {
        var keep = new Permission { Id = 10, Name = "users.read" };
        var remove = new Permission { Id = 20, Name = "users.write" };
        var add = new Permission { Id = 30, Name = "users.delete" };
        var role = new Role { Id = 1, Name = "Admin", Permissions = [keep, remove] };

        _repository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);
        _permissionRepository.ListAsync(Arg.Any<PermissionsByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([add]);

        var result = await _handler.Handle(
            new UpdateRoleCommand(1, "Admin",
                [new RolePermissionOperationDto(30, "add"), new RolePermissionOperationDto(20, "remove")]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        role.Permissions.Select(p => p.Id).ShouldBe([10, 30], ignoreOrder: true);
    }

    [Fact]
    public async Task ThrowsForUnknownOperation()
    {
        var role = new Role { Id = 1, Name = "Admin", Permissions = [] };

        _repository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);

        await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(
            new UpdateRoleCommand(1, "Admin", [new RolePermissionOperationDto(10, "bogus")]),
            CancellationToken.None));
    }
}
