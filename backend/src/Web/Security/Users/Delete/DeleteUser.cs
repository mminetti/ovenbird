using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Users.Delete;
using Web.Extensions;

namespace Web.Security.Users.Delete;

public class DeleteUser(IMessageBus bus)
    : Endpoint<DeleteUserRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeleteUserRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Delete a user";
            s.Description = "Deletes an existing user by ID. This action cannot be undone.";
            s.ExampleRequest = new DeleteUserRequest { UserId = 1 };

            s.Responses[204] = "User deleted successfully";
            s.Responses[404] = "User not found";
            s.Responses[400] = "Invalid request or deletion failed";
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<DeleteUserRequest>()
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(400));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(DeleteUserRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new DeleteUserCommand(request.UserId), ct);

        return result.ToDeleteResult();
    }
}
