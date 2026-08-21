using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using UseCases.Security.Users;
using UseCases.Security.Users.GetCurrentUser;
using Web.Configurations.Auth;
using Web.Resources;

namespace Web.Security.Users.Me;

public class GetMe(IMessageBus bus, ICurrentUserContext currentUser)
    : Endpoint<EmptyRequest,
               Results<Ok<MeRecord>, NotFound, UnauthorizedHttpResult, ProblemHttpResult>,
               GetMeMapper>
{
    private const string Route = "/security/me";

    public override void Configure()
    {
        Get(Route);

        Summary(s =>
        {
            s.Summary = "Get current user";
            s.Description = "Retrieves the authenticated user's profile, including their assigned permissions.";

            s.Responses[200] = Endpoints.Response200Ok;
            s.Responses[401] = Endpoints.Response401Unauthorized;
            s.Responses[404] = Endpoints.Response404NotFound;
            s.Responses[500] = Endpoints.Response500InternalServerError;
        });

        Tags("Security");

        Description(builder => builder
            .Accepts<EmptyRequest>()
            .Produces<MeRecord>(200, "application/json")
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(500));
    }

    public override async Task<Results<Ok<MeRecord>, NotFound, UnauthorizedHttpResult, ProblemHttpResult>>
        ExecuteAsync(EmptyRequest _, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<CurrentUserProfileDto>>(
            new GetCurrentUserQuery(currentUser.ExternalIdentifier!), ct);

        if (result.Status == ResultStatus.NotFound)
        {
            return TypedResults.NotFound();
        }

        if (result.Status == ResultStatus.Unauthorized)
        {
            return TypedResults.Unauthorized();
        }

        if (!result.IsSuccess)
        {
            return TypedResults.Problem(
                    title: "Get failed",
                    detail: string.Join("; ", result.Errors),
                    statusCode: StatusCodes.Status400BadRequest);
        }

        return TypedResults.Ok(Map.FromEntity(result.Value));
    }
}

public sealed class GetMeMapper : Mapper<EmptyRequest, MeRecord, CurrentUserProfileDto>
{
    public override MeRecord FromEntity(CurrentUserProfileDto dto)
    {
        var permissions = dto.Permissions
            .Select(p => new MePermissionRecord(p.Name, p.ModuleName))
            .ToList();

        return new MeRecord(dto.Name, dto.Email) { Permissions = permissions };
    }
}
