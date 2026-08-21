using Ardalis.Result;
using UseCases.Common;
using UseCases.Security.Permissions;
using UseCases.Security.Permissions.List;
using Web.Resources;

namespace Web.Security.Permissions.List;

public class ListPermissions(IMessageBus bus)
    : Endpoint<ListPermissionsRequest, ListPermissionsResponse, ListPermissionsMapper>
{
    private readonly IMessageBus _bus = bus;

    public override void Configure()
    {
        Get(ListPermissionsRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "List permissions";
            s.Description = "Retrieves a paginated list of all permissions.";
            s.ExampleRequest = new ListPermissionsRequest { Page = 1, PerPage = 10 };

            s.Params["page"] = Endpoints.ParamPage;
            s.Params["per_page"] = string.Format(Endpoints.ParamPerPage, Constants.MAX_PAGE_SIZE, Constants.DEFAULT_PAGE_SIZE);

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<ListPermissionsRequest>()
            .Produces<ListPermissionsResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(ListPermissionsRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemPagedResult<PermissionDto>>>(
            new ListPermissionsQuery(request.Page, request.PerPage), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, ct);
            return;
        }

        var response = Map.FromEntity(result.Value);

        await Send.OkAsync(response, ct);
    }
}

public sealed class ListPermissionsMapper
    : Mapper<ListPermissionsRequest, ListPermissionsResponse, ItemPagedResult<PermissionDto>>
{
    public override ListPermissionsResponse FromEntity(ItemPagedResult<PermissionDto> e)
    {
        var items = e.Items
            .Select(p => new PermissionRecord(p.Id, p.ModuleId, p.Name, p.Description))
            .ToList();

        return new ListPermissionsResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
