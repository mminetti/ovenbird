namespace UseCases.Security.Users.GetCurrentUser;

public class GetCurrentUserHandler(CurrentUserService currentUserService)
{
    public async Task<Result<CurrentUserProfileDto>> Handle(GetCurrentUserQuery query, CancellationToken ct)
    {
        var userInfo = await currentUserService.GetAsync(query.ExternalIdentifier, ct);

        if (userInfo is null)
        {
            return Result.NotFound();
        }

        if (!userInfo.IsActive)
        {
            return Result.Unauthorized();
        }

        return Result.Success(new CurrentUserProfileDto(userInfo.Name, userInfo.Email)
        {
            Permissions = [.. userInfo.Permissions.Select(p => new UserPermissionDto(p.Name, p.ModuleName))]
        });
    }
}
