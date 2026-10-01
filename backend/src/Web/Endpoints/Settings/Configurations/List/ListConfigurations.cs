using Ardalis.Result;
using UseCases.Common;
using UseCases.Common.Constants;
using UseCases.Settings.Configurations;
using UseCases.Settings.Configurations.List;
using Web.Resources;

namespace Web.Endpoints.Settings.Configurations.List;

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

            s.Params["page"] = EndpointSummaries.ParamPage;
            s.Params["per_page"] = string.Format(EndpointSummaries.ParamPerPage, Constants.Pagination.MaxPageSize, Constants.Pagination.DefaultPageSize);
            s.Params["search"] = string.Format(EndpointSummaries.ParamSearch, "name");
            s.Params["order_by"] = EndpointSummaries.ParamOrderBy;

            s.Responses[200] = EndpointSummaries.Response200Ok;
            s.Responses[400] = EndpointSummaries.Response400BadRequest;
            s.Responses[500] = EndpointSummaries.Response500InternalServerError;
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
                c.CompanyName,
                c.LastModifiedAtUtc,
                c.LastModifiedBy))
            .ToList();

        return new ListConfigurationsResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
