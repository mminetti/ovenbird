using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common.Constants;
using UseCases.Security.Roles.Create;
using Web.Extensions;
using Web.Resources;
using Web.Endpoints.Security.Roles.Get;

namespace Web.Endpoints.Security.Roles.Create;

public class CreateRole(IMessageBus bus)
    : Endpoint<CreateRoleRequest,
               Results<Created<CreateRoleResponse>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateRoleRequest.Route);
        Permissions(Constants.Permissions.RolesWrite);

        Summary(s =>
        {
            s.Summary = "Create a role";
            s.Description = "Creates a new role with the provided details.";
            s.ExampleRequest = new CreateRoleRequest { Name = "Admin", PermissionIds = [1, 2] };

            s.Responses[201] = EndpointSummaries.Response201Created;
            s.Responses[400] = EndpointSummaries.Response400BadRequest;
            s.Responses[500] = EndpointSummaries.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<CreateRoleRequest>("application/json")
            .Produces<CreateRoleResponse>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Created<CreateRoleResponse>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreateRoleRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<int>>(
            new CreateRoleCommand(request.Name, request.PermissionIds), ct);

        return result.ToCreatedResult(
            id => GetRoleRequest.BuildRoute(id),
            id => new CreateRoleResponse(id));
    }
}
