using Ardalis.Result;
using FluentValidation;
using UseCases.Security.Users;
using UseCases.Security.Users.List;

namespace Web.Security.Users.List;

public class ListUsers(IMessageBus bus) : Endpoint<ListUsersRequest, ListUsersResponse, ListUsersMapper>
{
    private readonly IMessageBus _bus = bus;
    public const string Route = "/security/users";

    public override void Configure()
    {
        Get(Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "List users";
            s.Description = "Retrieves a paginated list of all users.";
            s.ExampleRequest = new ListUsersRequest { Page = 1, PerPage = 10 };

            s.Params["page"] = "1-based page index (default 1)";
            s.Params["per_page"] = $"Page size 1–{UseCases.Constants.MAX_PAGE_SIZE} (default {UseCases.Constants.DEFAULT_PAGE_SIZE})";

            s.Responses[200] = "Paginated list of users returned successfully";
            s.Responses[400] = "Invalid pagination parameters";
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<ListUsersRequest>()
            .Produces<ListUsersResponse>(200, "application/json")
            .ProducesProblem(400));
    }

    public override async Task HandleAsync(ListUsersRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<UseCases.PagedResult<UserDto>>>(
            new ListUsersQuery(request.Page, request.PerPage), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, ct);
            return;
        }

        var response = Map.FromEntity(result.Value);

        await Send.OkAsync(response, ct);
    }
}

public sealed class ListUsersValidator : Validator<ListUsersRequest>
{
    public ListUsersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("page must be >= 1");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, UseCases.Constants.MAX_PAGE_SIZE)
            .WithMessage($"per_page must be between 1 and {UseCases.Constants.MAX_PAGE_SIZE}");
    }
}

public sealed class ListUsersMapper
    : Mapper<ListUsersRequest, ListUsersResponse, UseCases.PagedResult<UserDto>>
{
    public override ListUsersResponse FromEntity(UseCases.PagedResult<UserDto> e)
    {
        var items = e.Items
            .Select(u => new UserRecord(u.Id, u.Name, u.Email, u.IsActive))
            .ToList();

        return new ListUsersResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
