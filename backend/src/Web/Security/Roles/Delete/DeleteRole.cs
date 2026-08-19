using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Roles.Delete;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Roles.Delete;

public class DeleteRole(IMessageBus bus)
    : Endpoint<DeleteRoleRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeleteRoleRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Delete a role";
            s.Description = "Deletes an existing role by its unique identifier.";
            s.ExampleRequest = new DeleteRoleRequest { RoleId = 1 };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<DeleteRoleRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(DeleteRoleRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new DeleteRoleCommand(request.RoleId), ct);

        return result.ToDeleteUpdateResult();
    }
}
