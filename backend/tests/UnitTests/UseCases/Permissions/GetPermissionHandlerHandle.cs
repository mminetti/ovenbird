using Ardalis.SharedKernel;
using Core.Security;
using Core.Security.Specifications;
using UseCases.Security.Permissions;
using UseCases.Security.Permissions.Get;

namespace UnitTests.UseCases.Permissions;

public class GetPermissionHandlerHandle
{
    private readonly IReadRepository<Permission> _repository = Substitute.For<IReadRepository<Permission>>();
    private readonly GetPermissionHandler _handler;

    public GetPermissionHandlerHandle()
    {
        _handler = new GetPermissionHandler(_repository);
    }

    [Fact]
    public async Task ReturnsPermissionDtoWhenExists()
    {
        var permission = new Permission { Id = 1, ModuleId = 2, Name = "users.read", Description = "Can read users" };

        _repository.FirstOrDefaultAsync(Arg.Any<PermissionByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(permission);

        var result = await _handler.Handle(new GetPermissionQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new PermissionDto(1, 2, "users.read", "Can read users"));
    }

    [Fact]
    public async Task ReturnsNotFoundWhenPermissionDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<PermissionByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Permission?)null);

        var result = await _handler.Handle(new GetPermissionQuery(999), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
    }
}
