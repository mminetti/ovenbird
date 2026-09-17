using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Connectors.Delete;
using Web.Extensions;
using Web.Resources;

namespace Web.Connectors.Delete;

public class DeleteConnector(IMessageBus bus)
    : Endpoint<DeleteConnectorRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeleteConnectorRequest.Route);
        Permissions(Constants.Permissions.ConnectorsWrite);

        Summary(s =>
        {
            s.Summary = "Delete a connector";
            s.Description = "Deletes an existing connector by its unique identifier.";
            s.ExampleRequest = new DeleteConnectorRequest { ConnectorId = 1 };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Connectors");

        Description(builder => builder
            .Accepts<DeleteConnectorRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(DeleteConnectorRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new DeleteConnectorCommand(request.ConnectorId), ct);

        return result.ToDeleteUpdateResult();
    }
}
