using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common.Constants;
using UseCases.Settings.Connectors;
using UseCases.Settings.Connectors.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Endpoints.Settings.Connectors.Update;

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
                Fields = [new UpdateConnectorFieldRequest { Id = 1, Name = "host", Value = "localhost", Operation = "update" }]
            };

            s.Responses[204] = EndpointSummaries.Response200OkUpdated;
            s.Responses[400] = EndpointSummaries.Response400BadRequest;
            s.Responses[404] = EndpointSummaries.Response404NotFound;
            s.Responses[500] = EndpointSummaries.Response500InternalServerError;
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
            .Select(f => new UpdateConnectorFieldInput(f.Id, f.Name, f.Value, f.IsSecret, f.Operation))
            .ToList();

        var result = await bus.InvokeAsync<Result>(
            new UpdateConnectorCommand(request.ConnectorId, request.Name, request.Description, request.ConnectorImplementationId, fields), ct);

        return result.ToDeleteUpdateResult();
    }
}
