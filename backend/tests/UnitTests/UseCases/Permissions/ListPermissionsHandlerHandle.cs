using UseCases.Common;
using UseCases.Security.Permissions;
using UseCases.Security.Permissions.List;

namespace UnitTests.UseCases.Permissions;

public class ListPermissionsHandlerHandle
{
    private readonly IListPermissionsQueryService _query = Substitute.For<IListPermissionsQueryService>();
    private readonly ListPermissionsHandler _handler;

    public ListPermissionsHandlerHandle()
    {
        _handler = new ListPermissionsHandler(_query);
    }

    [Fact]
    public async Task ReturnsSuccessWithPagedResults()
    {
        var permissions = new List<PermissionDto>
        {
            new(1, 1, "users.read", "Can read users"),
            new(2, 1, "users.write", "Can write users"),
        };
        var pagedResult = new ItemPagedResult<PermissionDto>(permissions, 1, 10, 2, 1);

        _query.ListAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _handler.Handle(new ListPermissionsQuery(1, 10), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.TotalCount.ShouldBe(2);
        result.Value.Page.ShouldBe(1);
    }

    [Fact]
    public async Task UsesDefaultsWhenPageAndPerPageAreNull()
    {
        var pagedResult = new ItemPagedResult<PermissionDto>([], 1, Constants.DEFAULT_PAGE_SIZE, 0, 0);

        _query.ListAsync(1, Constants.DEFAULT_PAGE_SIZE, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _handler.Handle(new ListPermissionsQuery(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _query.Received(1).ListAsync(1, Constants.DEFAULT_PAGE_SIZE, Arg.Any<CancellationToken>());
    }
}
