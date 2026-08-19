using UseCases;
using UseCases.Security.Roles;
using UseCases.Security.Roles.List;

namespace UnitTests.UseCases.Roles;

public class ListRolesHandlerHandle
{
    private readonly IListRolesQueryService _query = Substitute.For<IListRolesQueryService>();
    private readonly ListRolesHandler _handler;

    public ListRolesHandlerHandle()
    {
        _handler = new ListRolesHandler(_query);
    }

    [Fact]
    public async Task ReturnsSuccessWithPagedResults()
    {
        var roles = new List<RoleDto>
        {
            new(1, "Admin"),
            new(2, "Editor"),
        };
        var pagedResult = new global::UseCases.PagedResult<RoleDto>(roles, 1, 10, 2, 1);

        _query.ListAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _handler.Handle(new ListRolesQuery(1, 10), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.TotalCount.ShouldBe(2);
        result.Value.Page.ShouldBe(1);
    }

    [Fact]
    public async Task UsesDefaultsWhenPageAndPerPageAreNull()
    {
        var pagedResult = new global::UseCases.PagedResult<RoleDto>([], 1, Constants.DEFAULT_PAGE_SIZE, 0, 0);

        _query.ListAsync(1, Constants.DEFAULT_PAGE_SIZE, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _handler.Handle(new ListRolesQuery(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _query.Received(1).ListAsync(1, Constants.DEFAULT_PAGE_SIZE, Arg.Any<CancellationToken>());
    }
}
