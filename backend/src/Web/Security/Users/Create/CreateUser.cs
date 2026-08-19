using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Users.Create;
using Web.Extensions;
using Web.Resources;
using Web.Security.Users.Get;

namespace Web.Security.Users.Create;

public class CreateUser(IMessageBus bus)
    : Endpoint<CreateUserRequest,
               Results<Created<CreateUserResponse>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateUserRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Create a user";
            s.Description = "Creates a new user with the provided details.";
            s.ExampleRequest = new CreateUserRequest
            {
                Name = "Alice",
                Email = "alice@example.com",
                ExternalIdentifier = "ext-alice"
            };

            s.Responses[201] = Endpoints.Response201Created;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<CreateUserRequest>("application/json")
            .Produces<CreateUserResponse>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Created<CreateUserResponse>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreateUserRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<int>>(
            new CreateUserCommand(request.Name, request.Email, request.ExternalIdentifier), ct);

        return result.ToCreatedResult(
            id => GetUserByIdRequest.BuildRoute(id),
            id => new CreateUserResponse(id, request.Name, request.Email));
    }
}
