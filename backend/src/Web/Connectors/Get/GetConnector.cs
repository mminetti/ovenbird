using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Connectors;
using UseCases.Connectors.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Connectors.Get;

public class GetConnector(IMessageBus bus)
    : Endpoint<GetConnectorRequest,
               Results<Ok<ConnectorRecord>, NotFound, ProblemHttpResult>,
               GetConnectorMapper>
{
    public override void Configure()
    {
        Get(GetConnectorRequest.Route);
        Permissions(Constants.Permissions.ConnectorsRead);

        Summary(s =>
        {
            s.Summary = "Get a connector";
            s.Description = "Retrieves a specific connector by its unique identifier.";
            s.ExampleRequest = new GetConnectorRequest { ConnectorId = 1 };

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Connectors");

        Description(builder => builder
            .Accepts<GetConnectorRequest>()
            .Produces<ConnectorRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<ConnectorRecord>, NotFound, ProblemHttpResult>>
        ExecuteAsync(GetConnectorRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<ConnectorDto>>(new GetConnectorQuery(request.ConnectorId), ct);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}

public sealed class GetConnectorMapper : Mapper<GetConnectorRequest, ConnectorRecord, ConnectorDto>
{
    public override ConnectorRecord FromEntity(ConnectorDto e)
    {
        var fields = e.Fields
            .Select(f => new ConnectorFieldRecord(f.Id, f.Name, f.Value, f.IsSecret))
            .ToList();

        return new ConnectorRecord(
            e.Id,
            e.Name,
            e.Description,
            e.ConnectorTypeId,
            e.ConnectorTypeName,
            e.ConnectorImplementationId,
            e.ConnectorImplementationName)
        {
            Fields = fields
        };
    }
}
