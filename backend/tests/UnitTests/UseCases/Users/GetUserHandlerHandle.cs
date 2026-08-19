using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Users.Get;

namespace UnitTests.UseCases.Users;

public class GetUserHandlerHandle
{
    private readonly IReadRepository<User> _repository = Substitute.For<IReadRepository<User>>();
    private readonly GetUserHandler _handler;

    public GetUserHandlerHandle()
    {
        _handler = new GetUserHandler(_repository);
    }

    [Fact]
    public async Task ReturnsSuccessWithRolesWhenUserExists()
    {
        var entity = new User
        {
            Id = 1,
            Name = "Alice",
            Email = "alice@example.com",
            IsActive = true,
            Roles = [new Role { Id = 5, Name = "Admin" }]
        };

        _repository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(entity);

        var result = await _handler.Handle(new GetUserQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(1);
        result.Value.Name.ShouldBe("Alice");
        result.Value.Roles.Count.ShouldBe(1);
        result.Value.Roles[0].Id.ShouldBe(5);
    }

    [Fact]
    public async Task ReturnsNotFoundWhenUserDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(new GetUserQuery(999), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
    }
}
