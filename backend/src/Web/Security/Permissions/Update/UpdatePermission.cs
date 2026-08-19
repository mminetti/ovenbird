using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Permissions.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Permissions.Update;

public class UpdatePermission(IMessageBus bus)
    : Endpoint<UpdatePermissionRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Put(UpdatePermissionRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Update a permission";
            s.Description = "Updates an existing permission with the provided details.";
            s.ExampleRequest = new UpdatePermissionRequest
            {
                PermissionId = 1,
                Id = 1,
                ModuleId = 1,
                Name = "users.write",
                Description = "Can write users"
            };

            s.Responses[204] = Endpoints.Response200OkUpdated;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<UpdatePermissionRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(UpdatePermissionRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(
            new UpdatePermissionCommand(request.PermissionId, request.ModuleId, request.Name, request.Description), ct);

        return result.ToDeleteUpdateResult();
    }
}
