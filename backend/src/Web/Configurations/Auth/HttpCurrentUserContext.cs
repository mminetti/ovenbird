using System.Security.Claims;

namespace Web.Configurations.Auth;

public class HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string? ExternalIdentifier => User?.GetExternalIdentifier();

    public string Name => User?.GetName() ?? string.Empty;

    public string Email => User?.GetEmail() ?? string.Empty;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;
}
