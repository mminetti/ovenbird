using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Markets;
using UseCases.Markets.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Markets.Get;

public class GetMarket(IMessageBus bus)
    : Endpoint<GetMarketRequest,
               Results<Ok<MarketRecord>, NotFound, ProblemHttpResult>,
               GetMarketMapper>
{
    public override void Configure()
    {
        Get(GetMarketRequest.Route);
        Permissions(Constants.Permissions.MarketsRead);

        Summary(s =>
        {
            s.Summary = "Get a market";
            s.Description = "Retrieves a specific market by its unique identifier.";
            s.ExampleRequest = new GetMarketRequest { MarketId = 1 };

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Markets");

        Description(builder => builder
            .Accepts<GetMarketRequest>()
            .Produces<MarketRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<MarketRecord>, NotFound, ProblemHttpResult>>
        ExecuteAsync(GetMarketRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<MarketDto>>(new GetMarketQuery(request.MarketId), ct);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}

public sealed class GetMarketMapper : Mapper<GetMarketRequest, MarketRecord, MarketDto>
{
    public override MarketRecord FromEntity(MarketDto e) =>
        new(e.Id, e.Name, e.Identifier, e.LastModifiedAtUtc, e.LastModifiedBy);
}
