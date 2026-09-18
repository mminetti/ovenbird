using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Markets.Create;
using Web.Extensions;
using Web.Markets.Get;
using Web.Resources;

namespace Web.Markets.Create;

public class CreateMarket(IMessageBus bus)
    : Endpoint<CreateMarketRequest,
               Results<Created<CreateMarketResponse>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateMarketRequest.Route);
        Permissions(Constants.Permissions.MarketsWrite);

        Summary(s =>
        {
            s.Summary = "Create a market";
            s.Description = "Creates a new market.";
            s.ExampleRequest = new CreateMarketRequest
            {
                Name = "PJM Interconnection",
                Identifier = "PJM"
            };

            s.Responses[201] = Endpoints.Response201Created;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Markets");

        Description(builder => builder
            .Accepts<CreateMarketRequest>("application/json")
            .Produces<CreateMarketResponse>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Created<CreateMarketResponse>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreateMarketRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<int>>(
            new CreateMarketCommand(request.Name, request.Identifier), ct);

        return result.ToCreatedResult(
            id => GetMarketRequest.BuildRoute(id),
            id => new CreateMarketResponse(id));
    }
}
