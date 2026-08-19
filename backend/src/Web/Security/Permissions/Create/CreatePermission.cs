using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Permissions.Create;
using Web.Extensions;
using Web.Resources;
using Web.Security.Permissions.Get;

namespace Web.Security.Permissions.Create;

public class CreatePermission(IMessageBus bus)
    : Endpoint<CreatePermissionRequest,
               Results<Created<CreatePermissionResponse>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreatePermissionRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Create a permission";
            s.Description = "Creates a new permission with the provided details.";
            s.ExampleRequest = new CreatePermissionRequest
            {
                ModuleId = 1,
                Name = "users.read",
                Description = "Can read users"
            };

            s.Responses[201] = Endpoints.Response201Created;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<CreatePermissionRequest>("application/json")
            .Produces<CreatePermissionResponse>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Created<CreatePermissionResponse>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreatePermissionRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<int>>(
            new CreatePermissionCommand(request.ModuleId, request.Name, request.Description), ct);

        return result.ToCreatedResult(
            id => GetPermissionRequest.BuildRoute(id),
            id => new CreatePermissionResponse(id));
    }
}
