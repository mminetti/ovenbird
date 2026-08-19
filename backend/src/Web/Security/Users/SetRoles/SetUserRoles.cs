using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Users.SetRoles;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Users.SetRoles;

public class SetUserRoles(IMessageBus bus)
    : Endpoint<SetUserRolesRequest,
               Results<NoContent, NotFound, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(SetUserRolesRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Set user roles";
            s.Description = "Replaces the user's role set with the supplied list.";
            s.ExampleRequest = new SetUserRolesRequest { UserId = 1, RoleIds = [1, 2] };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<SetUserRolesRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(SetUserRolesRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(
            new SetUserRolesCommand(request.UserId, request.RoleIds), ct);

        return result.ToNoContentResult();
    }
}
