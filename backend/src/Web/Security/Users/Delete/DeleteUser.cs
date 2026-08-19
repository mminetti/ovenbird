using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Users.Delete;
using Web.Extensions;
using Web.Resources;

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
            s.Description = "Deletes an existing user by their unique identifier.";
            s.ExampleRequest = new DeleteUserRequest { UserId = 1 };

            s.Responses[204] = Endpoints.Response204Deleted;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<DeleteUserRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
        ExecuteAsync(DeleteUserRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new DeleteUserCommand(request.UserId), ct);

        return result.ToDeleteResult();
    }
}
