using Core.Security;
using UseCases.Security.Users.Create;

namespace UnitTests.UseCases.Users;

public class CreateUserHandlerHandle
{
    private readonly IRepository<User> _repository = Substitute.For<IRepository<User>>();
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerHandle()
    {
        _handler = new CreateUserHandler(_repository);
    }

    [Fact]
    public async Task ReturnsSuccessWithNewId()
    {
        var created = new User { Id = 42, Name = "Alice", Email = "alice@example.com", ExternalIdentifier = "ext-1", IsActive = true };

        _repository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(created);

        var result = await _handler.Handle(
            new CreateUserCommand("Alice", "alice@example.com", "ext-1"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task SetsIsActiveToTrue()
    {
        User? captured = null;
        _repository.AddAsync(Arg.Do<User>(u => captured = u), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<User>());

        await _handler.Handle(
            new CreateUserCommand("Bob", "bob@example.com", "ext-2"),
            CancellationToken.None);

        captured.ShouldNotBeNull();
        captured!.IsActive.ShouldBeTrue();
        captured.Name.ShouldBe("Bob");
        captured.Email.ShouldBe("bob@example.com");
    }
}
