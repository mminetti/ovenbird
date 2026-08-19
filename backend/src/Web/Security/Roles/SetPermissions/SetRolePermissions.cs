using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Roles.SetPermissions;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Roles.SetPermissions;

public class SetRolePermissions(IMessageBus bus)
    : Endpoint<SetRolePermissionsRequest,
               Results<NoContent, NotFound, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(SetRolePermissionsRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Set role permissions";
            s.Description = "Replaces the role's permission set with the supplied list.";
            s.ExampleRequest = new SetRolePermissionsRequest { RoleId = 1, PermissionIds = [1, 2, 3] };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<SetRolePermissionsRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(SetRolePermissionsRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(
            new SetRolePermissionsCommand(request.RoleId, request.PermissionIds), ct);

        return result.ToNoContentResult();
    }
}
