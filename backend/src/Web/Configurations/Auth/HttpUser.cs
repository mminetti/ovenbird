using UseCases.Common;

namespace Web.Configurations.Auth;

public class HttpUser(ICurrentUserContext currentUserContext) : IUser
{
    public string? Id => currentUserContext.IsAuthenticated && !string.IsNullOrWhiteSpace(currentUserContext.Email)
        ? currentUserContext.Email
        : null;
}
