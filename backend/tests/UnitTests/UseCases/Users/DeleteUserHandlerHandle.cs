using Core.Security;
using UseCases.Security.Users.Delete;

namespace UnitTests.UseCases.Users;

public class DeleteUserHandlerHandle
{
    private readonly IRepository<User> _repository = Substitute.For<IRepository<User>>();
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerHandle()
    {
        _handler = new DeleteUserHandler(_repository);
    }

    [Fact]
    public async Task ReturnsSuccessWhenUserExists()
    {
        var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com" };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _handler.Handle(new DeleteUserCommand(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _repository.Received(1).DeleteAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenUserDoesNotExist()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(new DeleteUserCommand(999), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
