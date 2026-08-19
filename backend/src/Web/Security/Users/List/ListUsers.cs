using Ardalis.Result;
using UseCases.Security.Users;
using UseCases.Security.Users.List;
using Web.Resources;

namespace Web.Security.Users.List;

public class ListUsers(IMessageBus bus) : Endpoint<ListUsersRequest, ListUsersResponse, ListUsersMapper>
{
    private readonly IMessageBus _bus = bus;
    
    public override void Configure()
    {
        Get(ListUsersRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "List users";
            s.Description = "Retrieves a paginated list of all users.";
            s.ExampleRequest = new ListUsersRequest { Page = 1, PerPage = 10 };

            s.Params["page"] = Endpoints.ParamPage;
            s.Params["per_page"] = string.Format(Endpoints.ParamPerPage, UseCases.Constants.MAX_PAGE_SIZE, UseCases.Constants.DEFAULT_PAGE_SIZE);

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<ListUsersRequest>()
            .Produces<ListUsersResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
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
