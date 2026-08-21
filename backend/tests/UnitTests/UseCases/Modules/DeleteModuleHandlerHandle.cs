using Ardalis.Result;
using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Modules.Delete;

namespace UnitTests.UseCases.Modules;

public class DeleteModuleHandlerHandle
{
    private readonly IRepository<Module> _repository = Substitute.For<IRepository<Module>>();
    private readonly DeleteModuleHandler _handler;

    public DeleteModuleHandlerHandle()
    {
        _handler = new DeleteModuleHandler(_repository);
    }

    [Fact]
    public async Task ReturnsNotFoundWhenModuleDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<ModuleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Module?)null);

        var result = await _handler.Handle(new DeleteModuleCommand(999), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Module>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsConflictWhenModuleHasPermissions()
    {
        var module = new Module { Id = 1, Name = "Users" };
        module.Permissions.Add(new Permission { Id = 1, ModuleId = 1, Name = "users.read", Description = "Can read users" });

        _repository.FirstOrDefaultAsync(Arg.Any<ModuleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(module);

        var result = await _handler.Handle(new DeleteModuleCommand(1), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Module>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsSuccessWhenModuleHasNoPermissions()
    {
        var module = new Module { Id = 1, Name = "Users" };

        _repository.FirstOrDefaultAsync(Arg.Any<ModuleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(module);

        var result = await _handler.Handle(new DeleteModuleCommand(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _repository.Received(1).DeleteAsync(module, Arg.Any<CancellationToken>());
    }
}
