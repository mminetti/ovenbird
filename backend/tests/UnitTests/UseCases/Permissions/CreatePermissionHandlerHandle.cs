using Core.Security;
using UseCases.Security.Permissions.Create;

namespace UnitTests.UseCases.Permissions;

public class CreatePermissionHandlerHandle
{
    private readonly IRepository<Permission> _repository = Substitute.For<IRepository<Permission>>();
    private readonly CreatePermissionHandler _handler;

    public CreatePermissionHandlerHandle()
    {
        _handler = new CreatePermissionHandler(_repository);
    }

    [Fact]
    public async Task ReturnsSuccessWithNewId()
    {
        var created = new Permission { Id = 5, ModuleId = 1, Name = "users.read", Description = "Can read users" };

        _repository.AddAsync(Arg.Any<Permission>(), Arg.Any<CancellationToken>())
            .Returns(created);

        var result = await _handler.Handle(
            new CreatePermissionCommand(1, "users.read", "Can read users"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public async Task SetsFieldsCorrectly()
    {
        Permission? captured = null;
        _repository.AddAsync(Arg.Do<Permission>(p => captured = p), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<Permission>());

        await _handler.Handle(
            new CreatePermissionCommand(2, "users.write", "Can write users"),
            CancellationToken.None);

        captured.ShouldNotBeNull();
        captured!.ModuleId.ShouldBe(2);
        captured.Name.ShouldBe("users.write");
        captured.Description.ShouldBe("Can write users");
    }
}
