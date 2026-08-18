using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Users;
using UseCases.Security.Users.Get;
using Web.Extensions;

namespace Web.Security.Users.GetById;

public class GetUserById(IMessageBus bus)
    : Endpoint<GetUserByIdRequest,
               Results<Ok<UserRecord>, NotFound, ProblemHttpResult>,
               GetUserByIdMapper>
{
    public override void Configure()
    {
        Get(GetUserByIdRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Get a user";
            s.Description = "Retrieves a specific user by their unique identifier.";
            s.ExampleRequest = new GetUserByIdRequest { UserId = 1 };
            s.ResponseExamples[200] = new UserRecord(1, "Alice", "alice@example.com", true);

            s.Responses[200] = "User found and returned successfully";
            s.Responses[404] = "User with specified ID not found";
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<GetUserByIdRequest>()
            .Produces<UserRecord>(200, "application/json")
            .ProducesProblem(404));
    }

    public override async Task<Results<Ok<UserRecord>, NotFound, ProblemHttpResult>>
        ExecuteAsync(GetUserByIdRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<UserDto>>(new GetUserQuery(request.UserId), ct);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}

public sealed class GetUserByIdMapper : Mapper<GetUserByIdRequest, UserRecord, UserDto>
{
    public override UserRecord FromEntity(UserDto e) => new(e.Id, e.Name, e.Email, e.IsActive);
}
