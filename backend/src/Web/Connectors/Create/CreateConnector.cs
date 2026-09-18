using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Connectors;
using UseCases.Connectors.Create;
using Web.Connectors.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Connectors.Create;

public class CreateConnector(IMessageBus bus)
    : Endpoint<CreateConnectorRequest,
               Results<Created<CreateConnectorResponse>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateConnectorRequest.Route);
        Permissions(Constants.Permissions.ConnectorsWrite);

        Summary(s =>
        {
            s.Summary = "Create a connector";
            s.Description = "Creates a new connector with the provided type, implementation, and fields.";
            s.ExampleRequest = new CreateConnectorRequest
            {
                Name = "Market data drop folder",
                ConnectorImplementationId = 3,
                Fields = [new ConnectorFieldRequest { Name = "host", Value = "localhost" }]
            };

            s.Responses[201] = Endpoints.Response201Created;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Connectors");

        Description(builder => builder
            .Accepts<CreateConnectorRequest>("application/json")
            .Produces<CreateConnectorResponse>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Created<CreateConnectorResponse>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreateConnectorRequest request, CancellationToken ct)
    {
        var fields = request.Fields
            .Select(f => new ConnectorFieldInput(f.Name, f.Value, f.IsSecret))
            .ToList();

        var result = await bus.InvokeAsync<Result<int>>(
            new CreateConnectorCommand(request.Name, request.Description, request.ConnectorImplementationId, fields), ct);

        return result.ToCreatedResult(
            id => GetConnectorRequest.BuildRoute(id),
            id => new CreateConnectorResponse(id));
    }
}
