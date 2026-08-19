using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Roles;
using UseCases.Security.Roles.Get;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Roles.Get;

public class GetRole(IMessageBus bus)
    : Endpoint<GetRoleRequest,
               Results<Ok<RoleRecord>, NotFound, ProblemHttpResult>,
               GetRoleByIdMapper>
{
    public override void Configure()
    {
        Get(GetRoleRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Get a role";
            s.Description = "Retrieves a specific role by its unique identifier.";
            s.ExampleRequest = new GetRoleRequest { RoleId = 1 };

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<GetRoleRequest>()
            .Produces<RoleRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<RoleRecord>, NotFound, ProblemHttpResult>>
        ExecuteAsync(GetRoleRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<RoleDto>>(new GetRoleQuery(request.RoleId), ct);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}

public sealed class GetRoleByIdMapper : Mapper<GetRoleRequest, RoleRecord, RoleDto>
{
    public override RoleRecord FromEntity(RoleDto e) => new(e.Id, e.Name);
}
