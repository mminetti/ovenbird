using Ardalis.SharedKernel;
using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Roles;
using UseCases.Security.Roles.Get;

namespace UnitTests.UseCases.Roles;

public class GetRoleHandlerHandle
{
    private readonly IReadRepository<Role> _repository = Substitute.For<IReadRepository<Role>>();
    private readonly GetRoleHandler _handler;

    public GetRoleHandlerHandle()
    {
        _handler = new GetRoleHandler(_repository);
    }

    [Fact]
    public async Task ReturnsRoleDtoWhenExists()
    {
        var role = new Role { Id = 1, Name = "Admin" };

        _repository.FirstOrDefaultAsync(Arg.Any<RoleByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);

        var result = await _handler.Handle(new GetRoleQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new RoleDto(1, "Admin"));
    }

    [Fact]
    public async Task ReturnsNotFoundWhenRoleDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<RoleByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Role?)null);

        var result = await _handler.Handle(new GetRoleQuery(999), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
    }
}
