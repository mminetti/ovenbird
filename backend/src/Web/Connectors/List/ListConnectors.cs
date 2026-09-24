using Ardalis.Result;
using UseCases.Common;
using UseCases.Connectors;
using UseCases.Connectors.List;
using Web.Resources;

namespace Web.Connectors.List;

public class ListConnectors(IMessageBus bus) : Endpoint<ListConnectorsRequest, ListConnectorsResponse, ListConnectorsMapper>
{
    private readonly IMessageBus _bus = bus;

    public override void Configure()
    {
        Get(ListConnectorsRequest.Route);
        Permissions(Constants.Permissions.ConnectorsRead);

        Summary(s =>
        {
            s.Summary = "List connectors";
            s.Description = "Retrieves a paginated list of all connectors.";
            s.ExampleRequest = new ListConnectorsRequest { Page = 1, PerPage = 10 };

            s.Params["page"] = Endpoints.ParamPage;
            s.Params["per_page"] = string.Format(Endpoints.ParamPerPage, Constants.Pagination.MaxPageSize, Constants.Pagination.DefaultPageSize);
            s.Params["search"] = string.Format(Endpoints.ParamSearch, "name");
            s.Params["order_by"] = Endpoints.ParamOrderBy;

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Connectors");

        Description(builder => builder
            .Accepts<ListConnectorsRequest>()
            .Produces<ListConnectorsResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(ListConnectorsRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemPagedResult<ConnectorDto>>>(
            new ListConnectorsQuery(request.Page, request.PerPage, request.Search, request.OrderBy), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, ct);
            return;
        }

        var response = Map.FromEntity(result.Value);

        await Send.OkAsync(response, ct);
    }
}

public sealed class ListConnectorsMapper
    : Mapper<ListConnectorsRequest, ListConnectorsResponse, ItemPagedResult<ConnectorDto>>
{
    public override ListConnectorsResponse FromEntity(ItemPagedResult<ConnectorDto> e)
    {
        var items = e.Items
            .Select(c => new ConnectorRecord(
                c.Id,
                c.Name,
                c.Description,
                c.ConnectorTypeId,
                c.ConnectorTypeName,
                c.ConnectorImplementationId,
                c.ConnectorImplementationName,
                c.LastModifiedAtUtc,
                c.LastModifiedBy))
            .ToList();

        return new ListConnectorsResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
