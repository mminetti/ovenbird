using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Users;
using UseCases.Security.Users.Update;
using Web.Extensions;
using Web.Resources;

namespace Web.Security.Users.Update;

public class UpdateUser(IMessageBus bus)
    : Endpoint<UpdateUserRequest,
               Results<Ok<UpdateUserResponse>, NotFound, ProblemHttpResult>,
               UpdateUserMapper>
{
    public override void Configure()
    {
        Put(UpdateUserRequest.Route);
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Update a user";
            s.Description = "Updates an existing user with the provided details.";
            s.ExampleRequest = new UpdateUserRequest { UserId = 1, Id = 1, Name = "Alice Updated", Email = "alice@example.com", IsActive = true };
            
            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[400] = Endpoints.Response400BadRequest;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<UpdateUserRequest>("application/json")
            .Produces<UpdateUserResponse>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<UpdateUserResponse>, NotFound, ProblemHttpResult>>
        ExecuteAsync(UpdateUserRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<UserDto>>(
            new UpdateUserCommand(request.UserId, request.Name, request.Email, request.IsActive), ct);

        return result.ToUpdateResult(Map.FromEntity);
    }
}

public sealed class UpdateUserMapper : Mapper<UpdateUserRequest, UpdateUserResponse, UserDto>
{
    public override UpdateUserResponse FromEntity(UserDto e)
        => new(new UserRecord(e.Id, e.Name, e.Email, e.IsActive));
}
