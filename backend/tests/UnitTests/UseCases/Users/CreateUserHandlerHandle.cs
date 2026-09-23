using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Users.Create;

namespace UnitTests.UseCases.Users;

public class CreateUserHandlerHandle
{
    private readonly IRepository<User> _repository = Substitute.For<IRepository<User>>();
    private readonly IRepository<Role> _roleRepository = Substitute.For<IRepository<Role>>();
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerHandle()
    {
        _handler = new CreateUserHandler(_repository, _roleRepository);
    }

    [Fact]
    public async Task ReturnsSuccessWithNewId()
    {
        var created = new User { Id = 42, Name = "Alice", Email = "alice@example.com", ExternalIdentifier = "ext-1", IsActive = true };

        _repository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(created);

        var result = await _handler.Handle(
            new CreateUserCommand("Alice", "alice@example.com", "ext-1", []),
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
            new CreateUserCommand("Bob", "bob@example.com", "ext-2", []),
            CancellationToken.None);

        captured.ShouldNotBeNull();
        captured!.IsActive.ShouldBeTrue();
        captured.Name.ShouldBe("Bob");
        captured.Email.ShouldBe("bob@example.com");
    }

    [Fact]
    public async Task AssignsRolesWhenRoleIdsProvided()
    {
        var roles = new List<Role>
        {
            new() { Id = 10, Name = "Admin" },
            new() { Id = 20, Name = "Editor" }
        };

        User? captured = null;
        _roleRepository.ListAsync(Arg.Any<RolesByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns(roles);
        _repository.AddAsync(Arg.Do<User>(u => captured = u), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<User>());

        await _handler.Handle(
            new CreateUserCommand("Carol", "carol@example.com", "ext-3", [10, 20]),
            CancellationToken.None);

        captured.ShouldNotBeNull();
        captured!.Roles.Select(r => r.Id).ShouldBe([10, 20], ignoreOrder: true);
    }
}
