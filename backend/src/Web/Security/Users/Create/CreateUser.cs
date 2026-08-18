using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Users.Create;
using Web.Extensions;
using Web.Security.Users.GetById;

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
            s.Description = "Creates a new user with the provided name, email, and external identifier.";
            s.ExampleRequest = new CreateUserRequest
            {
                Name = "Alice",
                Email = "alice@example.com",
                ExternalIdentifier = "ext-alice"
            };
            s.ResponseExamples[201] = new CreateUserResponse(1, "Alice", "alice@example.com");

            s.Responses[201] = "User created successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[500] = "Internal server error";
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
