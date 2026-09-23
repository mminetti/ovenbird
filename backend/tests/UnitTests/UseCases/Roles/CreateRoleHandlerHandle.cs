using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Roles.Create;

namespace UnitTests.UseCases.Roles;

public class CreateRoleHandlerHandle
{
    private readonly IRepository<Role> _repository = Substitute.For<IRepository<Role>>();
    private readonly IRepository<Permission> _permissionRepository = Substitute.For<IRepository<Permission>>();
    private readonly CreateRoleHandler _handler;

    public CreateRoleHandlerHandle()
    {
        _handler = new CreateRoleHandler(_repository, _permissionRepository);
    }

    [Fact]
    public async Task ReturnsSuccessWithNewId()
    {
        var created = new Role { Id = 10, Name = "Admin" };

        _repository.AddAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>())
            .Returns(created);

        var result = await _handler.Handle(new CreateRoleCommand("Admin", []), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(10);
    }

    [Fact]
    public async Task SetsNameCorrectly()
    {
        Role? captured = null;
        _repository.AddAsync(Arg.Do<Role>(r => captured = r), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<Role>());

        await _handler.Handle(new CreateRoleCommand("Editor", []), CancellationToken.None);

        captured.ShouldNotBeNull();
        captured!.Name.ShouldBe("Editor");
    }

    [Fact]
    public async Task AssignsPermissionsWhenPermissionIdsProvided()
    {
        var permissions = new List<Permission>
        {
            new() { Id = 10, Name = "users.read", Description = "Can read users" },
            new() { Id = 20, Name = "users.write", Description = "Can write users" }
        };

        Role? captured = null;
        _permissionRepository.ListAsync(Arg.Any<PermissionsByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns(permissions);
        _repository.AddAsync(Arg.Do<Role>(r => captured = r), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<Role>());

        await _handler.Handle(new CreateRoleCommand("Viewer", [10, 20]), CancellationToken.None);

        captured.ShouldNotBeNull();
        captured!.Permissions.Select(p => p.Id).ShouldBe([10, 20], ignoreOrder: true);
    }
}
