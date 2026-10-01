using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Common.Constants;
using UseCases.Security.Users;
using UseCases.Security.Users.Get;
using Web.Extensions;
using Web.Resources;
using Web.Endpoints.Security.Roles;

namespace Web.Endpoints.Security.Users.Get;

public class GetUser(IMessageBus bus)
    : Endpoint<GetUserRequest,
               Results<Ok<UserRecord>, NotFound, ProblemHttpResult>,
               GetUserByIdMapper>
{
    public override void Configure()
    {
        Get(GetUserRequest.Route);
        Permissions(Constants.Permissions.UsersRead);

        Summary(s =>
        {
            s.Summary = "Get a user";
            s.Description = "Retrieves a specific user by their unique identifier.";
            s.ExampleRequest = new GetUserRequest { UserId = 1 };

            s.Responses[200] = EndpointSummaries.Response200Ok;
            s.Responses[400] = EndpointSummaries.Response400BadRequest;
            s.Responses[404] = EndpointSummaries.Response404NotFound;
            s.Responses[500] = EndpointSummaries.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<GetUserRequest>()
            .Produces<UserRecord>(200, "application/json")
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<UserRecord>, NotFound, ProblemHttpResult>>
        ExecuteAsync(GetUserRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<UserDto>>(new GetUserQuery(request.UserId), ct);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}

public sealed class GetUserByIdMapper : Mapper<GetUserRequest, UserRecord, UserDto>
{
    public override UserRecord FromEntity(UserDto e)
    {
        var roles = e.Roles
            .Select(r => new RoleRecord(r.Id, r.Name, r.LastModifiedAtUtc, r.LastModifiedBy))
            .ToList();

        return new UserRecord(e.Id, e.Name, e.Email, e.ExternalIdentifier, e.IsActive, e.LastModifiedAtUtc, e.LastModifiedBy) { Roles = roles };
    }
}
