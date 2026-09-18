using Ardalis.Result;
using UseCases.Common;
using UseCases.Configurations;
using UseCases.Configurations.List;
using Web.Resources;

namespace Web.Configurations.List;

public class ListConfigurations(IMessageBus bus) : Endpoint<ListConfigurationsRequest, ListConfigurationsResponse, ListConfigurationsMapper>
{
    private readonly IMessageBus _bus = bus;

    public override void Configure()
    {
        Get(ListConfigurationsRequest.Route);
        Permissions(Constants.Permissions.ConfigurationsRead);

        Summary(s =>
        {
            s.Summary = "List configurations";
            s.Description = "Retrieves a paginated list of all configurations.";
            s.ExampleRequest = new ListConfigurationsRequest { Page = 1, PerPage = 10 };

            s.Params["page"] = Endpoints.ParamPage;
            s.Params["per_page"] = string.Format(Endpoints.ParamPerPage, Constants.Pagination.MaxPageSize, Constants.Pagination.DefaultPageSize);
            s.Params["search"] = string.Format(Endpoints.ParamSearch, "name");
            s.Params["order_by"] = Endpoints.ParamOrderBy;

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Configurations");

        Description(builder => builder
            .Accepts<ListConfigurationsRequest>()
            .Produces<ListConfigurationsResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(ListConfigurationsRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemPagedResult<ConfigurationDto>>>(
            new ListConfigurationsQuery(request.Page, request.PerPage, request.Search, request.OrderBy), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, ct);
            return;
        }

        var response = Map.FromEntity(result.Value);

        await Send.OkAsync(response, ct);
    }
}

public sealed class ListConfigurationsMapper
    : Mapper<ListConfigurationsRequest, ListConfigurationsResponse, ItemPagedResult<ConfigurationDto>>
{
    public override ListConfigurationsResponse FromEntity(ItemPagedResult<ConfigurationDto> e)
    {
        var items = e.Items
            .Select(c => new ConfigurationRecord(
                c.Id,
                c.Name,
                c.Description,
                c.ConfigurationTypeId,
                c.ConfigurationTypeName,
                c.CompanyId,
                c.CompanyName))
            .ToList();

        return new ListConfigurationsResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
