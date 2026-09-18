using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Markets.Delete;
using Web.Extensions;
using Web.Resources;

namespace Web.Markets.Delete;

public class DeleteMarket(IMessageBus bus)
    : Endpoint<DeleteMarketRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeleteMarketRequest.Route);
        Permissions(Constants.Permissions.MarketsWrite);

        Summary(s =>
        {
            s.Summary = "Delete a market";
            s.Description = "Deletes an existing market by its unique identifier.";
            s.ExampleRequest = new DeleteMarketRequest { MarketId = 1 };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Markets");

        Description(builder => builder
            .Accepts<DeleteMarketRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(DeleteMarketRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new DeleteMarketCommand(request.MarketId), ct);

        return result.ToDeleteUpdateResult();
    }
}
