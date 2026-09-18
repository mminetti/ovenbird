using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Connectors;
using UseCases.Connectors.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Connectors.Update;

public class UpdateConnector(IMessageBus bus)
    : Endpoint<UpdateConnectorRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Put(UpdateConnectorRequest.Route);
        Permissions(Constants.Permissions.ConnectorsWrite);

        Summary(s =>
        {
            s.Summary = "Update a connector";
            s.Description = "Updates an existing connector's details, type, implementation, and fields.";
            s.ExampleRequest = new UpdateConnectorRequest
            {
                ConnectorId = 1,
                Id = 1,
                Name = "Market data drop folder",
                ConnectorImplementationId = 3,
                Fields = [new ConnectorFieldRequest { Name = "host", Value = "localhost" }]
            };

            s.Responses[204] = Endpoints.Response200OkUpdated;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Connectors");

        Description(builder => builder
            .Accepts<UpdateConnectorRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(UpdateConnectorRequest request, CancellationToken ct)
    {
        var fields = request.Fields
            .Select(f => new ConnectorFieldInput(f.Name, f.Value, f.IsSecret))
            .ToList();

        var result = await bus.InvokeAsync<Result>(
            new UpdateConnectorCommand(request.ConnectorId, request.Name, request.Description, request.ConnectorImplementationId, fields), ct);

        return result.ToDeleteUpdateResult();
    }
}
