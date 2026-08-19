using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Permissions.Delete;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Permissions.Delete;

public class DeletePermission(IMessageBus bus)
    : Endpoint<DeletePermissionRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeletePermissionRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Delete a permission";
            s.Description = "Deletes an existing permission by its unique identifier.";
            s.ExampleRequest = new DeletePermissionRequest { PermissionId = 1 };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<DeletePermissionRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(DeletePermissionRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new DeletePermissionCommand(request.PermissionId), ct);

        return result.ToDeleteUpdateResult();
    }
}
