using Core.Security;
using UseCases.Security.Permissions.Delete;

namespace UnitTests.UseCases.Permissions;

public class DeletePermissionHandlerHandle
{
    private readonly IRepository<Permission> _repository = Substitute.For<IRepository<Permission>>();
    private readonly DeletePermissionHandler _handler;

    public DeletePermissionHandlerHandle()
    {
        _handler = new DeletePermissionHandler(_repository);
    }

    [Fact]
    public async Task ReturnsSuccessWhenPermissionExists()
    {
        var permission = new Permission { Id = 1, ModuleId = 1, Name = "users.read", Description = "Can read users" };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(permission);

        var result = await _handler.Handle(new DeletePermissionCommand(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _repository.Received(1).DeleteAsync(permission, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenPermissionDoesNotExist()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Permission?)null);

        var result = await _handler.Handle(new DeletePermissionCommand(999), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Permission>(), Arg.Any<CancellationToken>());
    }
}
