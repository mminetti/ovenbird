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
    public async Task ReturnsRoleDtoWithPermissionsWhenExists()
    {
        var role = new Role
        {
            Id = 1,
            Name = "Admin",
            Permissions = [new Permission { Id = 10, ModuleId = 1, Name = "users.read", Description = "Can read users" }]
        };

        _repository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(role);

        var result = await _handler.Handle(new GetRoleQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(1);
        result.Value.Name.ShouldBe("Admin");
        result.Value.Permissions.Count.ShouldBe(1);
        result.Value.Permissions[0].Id.ShouldBe(10);
    }

    [Fact]
    public async Task ReturnsNotFoundWhenRoleDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<RoleWithPermissionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Role?)null);

        var result = await _handler.Handle(new GetRoleQuery(999), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
    }
}
