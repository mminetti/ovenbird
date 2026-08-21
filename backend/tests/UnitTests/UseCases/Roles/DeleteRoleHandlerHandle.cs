using Core.Security;
using NSubstitute;
using UseCases.Security.Roles.Delete;
using Wolverine;

namespace UnitTests.UseCases.Roles;

public class DeleteRoleHandlerHandle
{
    private readonly IRepository<Role> _repository = Substitute.For<IRepository<Role>>();
    private readonly IMessageBus _bus = Substitute.For<IMessageBus>();
    private readonly DeleteRoleHandler _handler;

    public DeleteRoleHandlerHandle()
    {
        _handler = new DeleteRoleHandler(_repository, _bus);
    }

    [Fact]
    public async Task ReturnsSuccessWhenRoleExists()
    {
        var role = new Role { Id = 1, Name = "Admin" };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(role);

        var result = await _handler.Handle(new DeleteRoleCommand(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _repository.Received(1).DeleteAsync(role, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenRoleDoesNotExist()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Role?)null);

        var result = await _handler.Handle(new DeleteRoleCommand(999), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>());
    }
}
