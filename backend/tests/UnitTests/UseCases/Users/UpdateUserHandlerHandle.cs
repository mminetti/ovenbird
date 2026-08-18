using Core.Security;
using UseCases.Security.Users.Update;

namespace UnitTests.UseCases.Users;

public class UpdateUserHandlerHandle
{
    private readonly IRepository<User> _repository = Substitute.For<IRepository<User>>();
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerHandle()
    {
        _handler = new UpdateUserHandler(_repository);
    }

    [Fact]
    public async Task ReturnsUpdatedUserWhenExists()
    {
        var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com", IsActive = true };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _handler.Handle(
            new UpdateUserCommand(1, "Alice Updated", "alice2@example.com", false),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Name.ShouldBe("Alice Updated");
        result.Value.Email.ShouldBe("alice2@example.com");
        result.Value.IsActive.ShouldBeFalse();
        await _repository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenUserDoesNotExist()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(
            new UpdateUserCommand(999, "Name", "email@example.com", true),
            CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
