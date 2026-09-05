using System.Security.Claims;

namespace Web.Configurations.Auth;

public class HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor, IAuthStrategy authStrategy) : ICurrentUserContext
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public string? ExternalIdentifier => IsAuthenticated ? authStrategy.GetExternalIdentifier(User!) : null;

    public string Name => IsAuthenticated ? authStrategy.GetName(User!) : string.Empty;

    public string Email => IsAuthenticated ? authStrategy.GetEmail(User!) : string.Empty;
}
