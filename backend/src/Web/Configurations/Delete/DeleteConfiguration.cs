using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Configurations.Delete;
using Web.Extensions;
using Web.Resources;

namespace Web.Configurations.Delete;

public class DeleteConfiguration(IMessageBus bus)
    : Endpoint<DeleteConfigurationRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeleteConfigurationRequest.Route);
        Permissions(Constants.Permissions.ConfigurationsWrite);

        Summary(s =>
        {
            s.Summary = "Delete a configuration";
            s.Description = "Deletes an existing configuration by its unique identifier.";
            s.ExampleRequest = new DeleteConfigurationRequest { ConfigurationId = 1 };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Configurations");

        Description(builder => builder
            .Accepts<DeleteConfigurationRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(DeleteConfigurationRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new DeleteConfigurationCommand(request.ConfigurationId), ct);

        return result.ToDeleteUpdateResult();
    }
}
