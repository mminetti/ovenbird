using Core.Security;
using UseCases.Security.Roles.Update;

namespace UnitTests.UseCases.Roles;

public class UpdateRoleHandlerHandle
{
    private readonly IRepository<Role> _repository = Substitute.For<IRepository<Role>>();
    private readonly UpdateRoleHandler _handler;

    public UpdateRoleHandlerHandle()
    {
        _handler = new UpdateRoleHandler(_repository);
    }

    [Fact]
    public async Task ReturnsSuccessWhenRoleExists()
    {
        var role = new Role { Id = 1, Name = "Admin" };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(role);

        var result = await _handler.Handle(new UpdateRoleCommand(1, "SuperAdmin"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        role.Name.ShouldBe("SuperAdmin");
        await _repository.Received(1).UpdateAsync(role, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsNotFoundWhenRoleDoesNotExist()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Role?)null);

        var result = await _handler.Handle(new UpdateRoleCommand(999, "Name"), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>());
    }
}
