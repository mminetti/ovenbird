using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common;
using UseCases.Security.Users.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Users.Update;

public class UpdateUser(IMessageBus bus)
    : Endpoint<UpdateUserRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Put(UpdateUserRequest.Route);
        Permissions(Constants.Permissions.UsersWrite);

        Summary(s =>
        {
            s.Summary = "Update a user";
            s.Description = "Updates an existing user with the provided details.";
            s.ExampleRequest = new UpdateUserRequest
            {
                UserId = 1,
                Id = 1,
                Name = "Alice Updated",
                Email = "alice@example.com",
                IsActive = true,
                Roles =
                [
                    new UpdateUserRoleRequest { RoleId = 2, Operation = "add" },
                    new UpdateUserRoleRequest { RoleId = 3, Operation = "remove" }
                ]
            };

            s.Responses[204] = Endpoints.Response200OkUpdated;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<UpdateUserRequest>("application/json")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(UpdateUserRequest request, CancellationToken ct)
    {
        var roles = request.Roles
            .Select(r => new UserRoleOperationDto(r.RoleId, r.Operation))
            .ToList();

        var result = await bus.InvokeAsync<Result>(
            new UpdateUserCommand(request.UserId, request.Name, request.Email, request.IsActive, roles), ct);

        return result.ToDeleteUpdateResult();
    }
}
