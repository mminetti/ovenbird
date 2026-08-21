using System.Security.Claims;
using Microsoft.Identity.Web;

namespace Web.Configurations.Auth;

public static class ClaimsPrincipalExtensions
{
    public static string? GetExternalIdentifier(this ClaimsPrincipal principal) =>
        principal.FindFirst(ClaimConstants.ObjectId)?.Value
        ?? principal.FindFirst(ClaimConstants.Oid)?.Value;

    public static string GetName(this ClaimsPrincipal principal) =>
        principal.FindFirst(ClaimTypes.Name)?.Value
        ?? principal.FindFirst(ClaimConstants.PreferredUserName)?.Value
        ?? string.Empty;

    public static string GetEmail(this ClaimsPrincipal principal) =>
        principal.FindFirst(ClaimTypes.Email)?.Value
        ?? principal.FindFirst(ClaimTypes.Upn)?.Value
        ?? string.Empty;
}
