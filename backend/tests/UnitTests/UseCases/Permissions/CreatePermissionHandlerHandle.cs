using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Permissions.Create;

namespace UnitTests.UseCases.Permissions;

public class CreatePermissionHandlerHandle
{
    private readonly IRepository<Permission> _repository = Substitute.For<IRepository<Permission>>();
    private readonly IReadRepository<Module> _moduleRepository = Substitute.For<IReadRepository<Module>>();
    private readonly CreatePermissionHandler _handler;

    public CreatePermissionHandlerHandle()
    {
        _handler = new CreatePermissionHandler(_repository, _moduleRepository);
    }

    [Fact]
    public async Task ReturnsSuccessWithNewId()
    {
        var module = new Module { Id = 1, Name = "Users" };
        var created = new Permission { Id = 5, ModuleId = 1, Name = "users.read", Description = "Can read users" };

        _moduleRepository.FirstOrDefaultAsync(Arg.Any<ModuleByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(module);
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
        var module = new Module { Id = 2, Name = "Orders" };
        Permission? captured = null;

        _moduleRepository.FirstOrDefaultAsync(Arg.Any<ModuleByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(module);
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

    [Fact]
    public async Task ReturnsInvalidWhenModuleDoesNotExist()
    {
        _moduleRepository.FirstOrDefaultAsync(Arg.Any<ModuleByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Module?)null);

        var result = await _handler.Handle(
            new CreatePermissionCommand(99, "users.read", "Can read users"),
            CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage == "Module not found.");
        await _repository.DidNotReceive().AddAsync(Arg.Any<Permission>(), Arg.Any<CancellationToken>());
    }
}
