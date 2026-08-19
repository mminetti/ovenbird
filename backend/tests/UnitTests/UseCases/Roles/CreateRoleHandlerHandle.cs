using Core.Security;
using UseCases.Security.Roles.Create;

namespace UnitTests.UseCases.Roles;

public class CreateRoleHandlerHandle
{
    private readonly IRepository<Role> _repository = Substitute.For<IRepository<Role>>();
    private readonly CreateRoleHandler _handler;

    public CreateRoleHandlerHandle()
    {
        _handler = new CreateRoleHandler(_repository);
    }

    [Fact]
    public async Task ReturnsSuccessWithNewId()
    {
        var created = new Role { Id = 10, Name = "Admin" };

        _repository.AddAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>())
            .Returns(created);

        var result = await _handler.Handle(new CreateRoleCommand("Admin"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(10);
    }

    [Fact]
    public async Task SetsNameCorrectly()
    {
        Role? captured = null;
        _repository.AddAsync(Arg.Do<Role>(r => captured = r), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<Role>());

        await _handler.Handle(new CreateRoleCommand("Editor"), CancellationToken.None);

        captured.ShouldNotBeNull();
        captured!.Name.ShouldBe("Editor");
    }
}
