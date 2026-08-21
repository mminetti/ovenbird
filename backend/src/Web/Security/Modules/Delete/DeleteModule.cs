using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Modules.Delete;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Modules.Delete;

public class DeleteModule(IMessageBus bus)
    : Endpoint<DeleteModuleRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeleteModuleRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Delete a module";
            s.Description = "Deletes an existing module by its unique identifier.";
            s.ExampleRequest = new DeleteModuleRequest { ModuleId = 1 };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[409] = Endpoints.Response409Conflict;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<DeleteModuleRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(409)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(DeleteModuleRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new DeleteModuleCommand(request.ModuleId), ct);

        return result.ToDeleteUpdateResult();
    }
}
