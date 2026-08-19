using Core.Security;
using UseCases.Security.Permissions.Update;

namespace UnitTests.UseCases.Permissions;

public class UpdatePermissionHandlerHandle
{
    private readonly IRepository<Permission> _repository = Substitute.For<IRepository<Permission>>();
    private readonly UpdatePermissionHandler _handler;

    public UpdatePermissionHandlerHandle()
    {
        _handler = new UpdatePermissionHandler(_repository);
    }

    [Fact]
    public async Task ReturnsSuccessWhenPermissionExists()
    {
        var permission = new Permission { Id = 1, ModuleId = 1, Name = "users.read", Description = "Can read users" };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(permission);

        var result = await _handler.Handle(
            new UpdatePermissionCommand(1, 2, "users.write", "Can write users"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        permission.ModuleId.ShouldBe(2);
        permission.Name.ShouldBe("users.write");
        permission.Description.ShouldBe("Can write users");
        await _repository.Received(1).UpdateAsync(permission, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenPermissionDoesNotExist()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Permission?)null);

        var result = await _handler.Handle(
            new UpdatePermissionCommand(999, 1, "users.read", "desc"),
            CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Permission>(), Arg.Any<CancellationToken>());
    }
}
