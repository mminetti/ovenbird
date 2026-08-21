using Ardalis.Result;
using UseCases.Common;
using UseCases.Security.Modules;
using UseCases.Security.Modules.List;
using Web.Resources;

namespace Web.Security.Modules.List;

public class ListModules(IMessageBus bus) : Endpoint<ListModulesRequest, ListModulesResponse, ListModulesMapper>
{
    private readonly IMessageBus _bus = bus;

    public override void Configure()
    {
        Get(ListModulesRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "List modules";
            s.Description = "Retrieves a paginated list of all modules.";
            s.ExampleRequest = new ListModulesRequest { Page = 1, PerPage = 10 };

            s.Params["page"] = Endpoints.ParamPage;
            s.Params["per_page"] = string.Format(Endpoints.ParamPerPage, Constants.MAX_PAGE_SIZE, Constants.DEFAULT_PAGE_SIZE);

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<ListModulesRequest>()
            .Produces<ListModulesResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(ListModulesRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemPagedResult<ModuleDto>>>(
            new ListModulesQuery(request.Page, request.PerPage), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, ct);
            return;
        }

        var response = Map.FromEntity(result.Value);

        await Send.OkAsync(response, ct);
    }
}

public sealed class ListModulesMapper
    : Mapper<ListModulesRequest, ListModulesResponse, ItemPagedResult<ModuleDto>>
{
    public override ListModulesResponse FromEntity(ItemPagedResult<ModuleDto> e)
    {
        var items = e.Items
            .Select(m => new ModuleRecord(m.Id, m.Name))
            .ToList();

        return new ListModulesResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
