using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Roles.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Roles.Update;

public class UpdateRole(IMessageBus bus)
    : Endpoint<UpdateRoleRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Put(UpdateRoleRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Update a role";
            s.Description = "Updates an existing role with the provided details.";
            s.ExampleRequest = new UpdateRoleRequest { RoleId = 1, Id = 1, Name = "Admin Updated" };

            s.Responses[204] = Endpoints.Response200OkUpdated;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<UpdateRoleRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(UpdateRoleRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(
            new UpdateRoleCommand(request.RoleId, request.Name), ct);

        return result.ToDeleteUpdateResult();
    }
}
