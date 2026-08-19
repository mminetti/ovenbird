using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Modules.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Modules.Update;

public class UpdateModule(IMessageBus bus)
    : Endpoint<UpdateModuleRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Put(UpdateModuleRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Update a module";
            s.Description = "Updates an existing module with the provided details.";
            s.ExampleRequest = new UpdateModuleRequest { ModuleId = 1, Id = 1, Name = "Administration Updated" };

            s.Responses[204] = Endpoints.Response200OkUpdated;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<UpdateModuleRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(UpdateModuleRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(
            new UpdateModuleCommand(request.ModuleId, request.Name), ct);

        return result.ToDeleteUpdateResult();
    }
}
