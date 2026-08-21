namespace UseCases.Security.Users.GetOrCreate;

public class GetOrCreateCurrentUserHandler(CurrentUserService currentUserService)
{
    public async Task<Result<CurrentUserInfo>> Handle(GetOrCreateCurrentUserCommand command, CancellationToken ct)
    {
        var userInfo = await currentUserService.GetOrCreateAsync(command.ExternalIdentifier, command.Name, command.Email, ct);

        if (!userInfo.IsActive)
        {
            return Result.Unauthorized();
        }

        return Result.Success(userInfo);
    }
}
