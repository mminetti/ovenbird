using Ardalis.SharedKernel;
using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Users.SetRoles;

namespace UnitTests.UseCases.Security.Users;

public class SetUserRolesHandlerHandle
{
    private readonly IRepository<User> _userRepository = Substitute.For<IRepository<User>>();
    private readonly IReadRepository<Role> _roleRepository = Substitute.For<IReadRepository<Role>>();
    private readonly SetUserRolesHandler _handler;

    public SetUserRolesHandlerHandle()
    {
        _handler = new SetUserRolesHandler(_userRepository, _roleRepository);
    }

    [Fact]
    public async Task ReturnsSuccessAndAssignsNewRoles()
    {
        var user = new User { Id = 1, Name = "Alice", Roles = [] };
        var roles = new List<Role>
        {
            new() { Id = 10, Name = "Admin" },
            new() { Id = 20, Name = "Editor" }
        };

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _roleRepository.ListAsync(Arg.Any<RolesByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns(roles);

        var result = await _handler.Handle(
            new SetUserRolesCommand(1, [10, 20]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        user.Roles.Count.ShouldBe(2);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenUserDoesNotExist()
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(
            new SetUserRolesCommand(999, [1]),
            CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ClearsAllRolesWhenListIsEmpty()
    {
        var user = new User { Id = 1, Name = "Alice", Roles = [new Role { Id = 10, Name = "Admin" }] };

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _handler.Handle(
            new SetUserRolesCommand(1, []),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        user.Roles.ShouldBeEmpty();
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DiffKeepsAssignedRemovesAndAddsCorrectly()
    {
        var keep = new Role { Id = 10, Name = "Admin" };
        var remove = new Role { Id = 20, Name = "Editor" };
        var user = new User { Id = 1, Name = "Alice", Roles = [keep, remove] };
        var add = new Role { Id = 30, Name = "Viewer" };

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserWithRolesByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _roleRepository.ListAsync(Arg.Any<RolesByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns(new List<Role> { keep, add });

        var result = await _handler.Handle(
            new SetUserRolesCommand(1, [10, 30]),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        user.Roles.Select(r => r.Id).ShouldBe([10, 30], ignoreOrder: true);
    }
}
