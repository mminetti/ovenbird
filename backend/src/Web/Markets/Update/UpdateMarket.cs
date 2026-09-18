using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Markets.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Markets.Update;

public class UpdateMarket(IMessageBus bus)
    : Endpoint<UpdateMarketRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Put(UpdateMarketRequest.Route);
        Permissions(Constants.Permissions.MarketsWrite);

        Summary(s =>
        {
            s.Summary = "Update a market";
            s.Description = "Updates an existing market's details.";
            s.ExampleRequest = new UpdateMarketRequest
            {
                MarketId = 1,
                Id = 1,
                Name = "PJM Interconnection",
                Identifier = "PJM"
            };

            s.Responses[204] = Endpoints.Response200OkUpdated;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Markets");

        Description(builder => builder
            .Accepts<UpdateMarketRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(UpdateMarketRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(
            new UpdateMarketCommand(request.MarketId, request.Name, request.Identifier), ct);

        return result.ToDeleteUpdateResult();
    }
}
