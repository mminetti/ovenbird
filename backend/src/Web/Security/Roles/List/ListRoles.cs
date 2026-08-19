using Ardalis.Result;
using UseCases.Security.Roles;
using UseCases.Security.Roles.List;
using Web.Resources;

namespace Web.Security.Roles.List;

public class ListRoles(IMessageBus bus) : Endpoint<ListRolesRequest, ListRolesResponse, ListRolesMapper>
{
    private readonly IMessageBus _bus = bus;

    public override void Configure()
    {
        Get(ListRolesRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "List roles";
            s.Description = "Retrieves a paginated list of all roles.";
            s.ExampleRequest = new ListRolesRequest { Page = 1, PerPage = 10 };

            s.Params["page"] = Endpoints.ParamPage;
            s.Params["per_page"] = string.Format(Endpoints.ParamPerPage, UseCases.Constants.MAX_PAGE_SIZE, UseCases.Constants.DEFAULT_PAGE_SIZE);

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<ListRolesRequest>()
            .Produces<ListRolesResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(ListRolesRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<UseCases.PagedResult<RoleDto>>>(
            new ListRolesQuery(request.Page, request.PerPage), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, ct);
            return;
        }

        var response = Map.FromEntity(result.Value);

        await Send.OkAsync(response, ct);
    }
}

public sealed class ListRolesMapper
    : Mapper<ListRolesRequest, ListRolesResponse, UseCases.PagedResult<RoleDto>>
{
    public override ListRolesResponse FromEntity(UseCases.PagedResult<RoleDto> e)
    {
        var items = e.Items
            .Select(r => new RoleRecord(r.Id, r.Name))
            .ToList();

        return new ListRolesResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
