using Ardalis.Result;
using UseCases.Common;
using UseCases.Markets;
using UseCases.Markets.List;
using Web.Resources;

namespace Web.Markets.List;

public class ListMarkets(IMessageBus bus) : Endpoint<ListMarketsRequest, ListMarketsResponse, ListMarketsMapper>
{
    private readonly IMessageBus _bus = bus;

    public override void Configure()
    {
        Get(ListMarketsRequest.Route);
        Permissions(Constants.Permissions.MarketsRead);

        Summary(s =>
        {
            s.Summary = "List markets";
            s.Description = "Retrieves a paginated list of all markets.";
            s.ExampleRequest = new ListMarketsRequest { Page = 1, PerPage = 10 };

            s.Params["page"] = Endpoints.ParamPage;
            s.Params["per_page"] = string.Format(Endpoints.ParamPerPage, Constants.Pagination.MaxPageSize, Constants.Pagination.DefaultPageSize);
            s.Params["search"] = string.Format(Endpoints.ParamSearch, "name");
            s.Params["order_by"] = Endpoints.ParamOrderBy;

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Markets");

        Description(builder => builder
            .Accepts<ListMarketsRequest>()
            .Produces<ListMarketsResponse>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task HandleAsync(ListMarketsRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemPagedResult<MarketDto>>>(
            new ListMarketsQuery(request.Page, request.PerPage, request.Search, request.OrderBy), ct);

        if (!result.IsSuccess)
        {
            await Send.ErrorsAsync(statusCode: 400, ct);
            return;
        }

        var response = Map.FromEntity(result.Value);

        await Send.OkAsync(response, ct);
    }
}

public sealed class ListMarketsMapper
    : Mapper<ListMarketsRequest, ListMarketsResponse, ItemPagedResult<MarketDto>>
{
    public override ListMarketsResponse FromEntity(ItemPagedResult<MarketDto> e)
    {
        var items = e.Items
            .Select(m => new MarketRecord(m.Id, m.Name, m.Identifier, m.LastModifiedAtUtc, m.LastModifiedBy))
            .ToList();

        return new ListMarketsResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
    }
}
