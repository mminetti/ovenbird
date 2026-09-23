using Core.Security;
using Core.Security.Specifications;
using NSubstitute;
using UseCases.Security.Users.Update;
using Wolverine;

namespace UnitTests.UseCases.Users;

public class UpdateUserHandlerHandle
{
    private readonly IRepository<User> _repository = Substitute.For<IRepository<User>>();
    private readonly IReadRepository<Role> _roleRepository = Substitute.For<IReadRepository<Role>>();
    private readonly IMessageBus _bus = Substitute.For<IMessageBus>();
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerHandle()
    {
        _handler = new UpdateUserHandler(_repository, _roleRepository, _bus);
    }

    [Fact]
    public async Task ReturnsUpdatedUserWhenExists()
    {
        var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com", IsActive = true };

        _repository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _handler.Handle(
            new UpdateUserCommand(1, "Alice Updated", "alice2@example.com", false, null),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _repository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenUserDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(
            new UpdateUserCommand(999, "Name", "email@example.com", true, null),
            CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddsRoleWhenOperationIsAdd()
    {
        var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com", IsActive = true, Roles = [] };
        var role = new Role { Id = 10, Name = "Admin" };

        _repository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _roleRepository.ListAsync(Arg.Any<RolesByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([role]);

        var result = await _handler.Handle(
            new UpdateUserCommand(1, "Alice", "alice@example.com", true, [new UserRoleOperationDto(10, "add")]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        user.Roles.Select(r => r.Id).ShouldBe([10]);
    }

    [Fact]
    public async Task RemovesRoleWhenOperationIsRemove()
    {
        var role = new Role { Id = 10, Name = "Admin" };
        var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com", IsActive = true, Roles = [role] };

        _repository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _handler.Handle(
            new UpdateUserCommand(1, "Alice", "alice@example.com", true, [new UserRoleOperationDto(10, "remove")]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        user.Roles.ShouldBeEmpty();
    }

    [Fact]
    public async Task LeavesOtherRolesUntouchedWhenAddingOrRemoving()
    {
        var keep = new Role { Id = 10, Name = "Admin" };
        var remove = new Role { Id = 20, Name = "Editor" };
        var add = new Role { Id = 30, Name = "Viewer" };
        var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com", IsActive = true, Roles = [keep, remove] };

        _repository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _roleRepository.ListAsync(Arg.Any<RolesByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([add]);

        var result = await _handler.Handle(
            new UpdateUserCommand(1, "Alice", "alice@example.com", true,
                [new UserRoleOperationDto(30, "add"), new UserRoleOperationDto(20, "remove")]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        user.Roles.Select(r => r.Id).ShouldBe([10, 30], ignoreOrder: true);
    }

    [Fact]
    public async Task ThrowsForUnknownOperation()
    {
        var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com", IsActive = true, Roles = [] };

        _repository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);

        await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(
            new UpdateUserCommand(1, "Alice", "alice@example.com", true, [new UserRoleOperationDto(10, "bogus")]),
            CancellationToken.None));
    }
}
