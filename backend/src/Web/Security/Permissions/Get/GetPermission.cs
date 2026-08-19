using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Permissions;
using UseCases.Security.Permissions.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Permissions.Get;

public class GetPermission(IMessageBus bus)
    : Endpoint<GetPermissionRequest,
               Results<Ok<PermissionRecord>, NotFound, ProblemHttpResult>,
               GetPermissionByIdMapper>
{
    public override void Configure()
    {
        Get(GetPermissionRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Get a permission";
            s.Description = "Retrieves a specific permission by its unique identifier.";
            s.ExampleRequest = new GetPermissionRequest { PermissionId = 1 };

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<GetPermissionRequest>()
            .Produces<PermissionRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<PermissionRecord>, NotFound, ProblemHttpResult>>
        ExecuteAsync(GetPermissionRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<PermissionDto>>(
            new GetPermissionQuery(request.PermissionId), ct);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}

public sealed class GetPermissionByIdMapper : Mapper<GetPermissionRequest, PermissionRecord, PermissionDto>
{
    public override PermissionRecord FromEntity(PermissionDto e) =>
        new(e.Id, e.ModuleId, e.Name, e.Description);
}
