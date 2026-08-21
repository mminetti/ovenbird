using System.Security.Claims;
using Ardalis.Result;
using Microsoft.AspNetCore.Authentication;
using UseCases.Security.Users.GetOrCreate;

namespace Web.Configurations.Auth;

public class CurrentUserClaimsTransformation(IMessageBus bus) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        // Claims transformation can run more than once per request. A dedicated marker claim
        // is used (rather than checking for the permissions claim) since a user with zero
        // permissions would otherwise be re-resolved on every invocation.
        if (principal.HasClaim(c => c.Type == AuthConstants.CurrentUserResolvedClaimType))
        {
            return principal;
        }

        var externalIdentifier = principal.GetExternalIdentifier();

        if (string.IsNullOrEmpty(externalIdentifier))
        {
            return principal;
        }

        var result = await bus.InvokeAsync<Result<CurrentUserInfo>>(
            new GetOrCreateCurrentUserCommand(externalIdentifier, principal.GetName(), principal.GetEmail()));

        if (result.Status == ResultStatus.Unauthorized)
        {
            throw new CurrentUserUnauthorizedException("Current user is not authorized.");
        }

        if (!result.IsSuccess)
        {
            throw new CurrentUserResolutionException("Failed to resolve current user.");
        }

        var identity = new ClaimsIdentity(principal.Identity);
        identity.AddClaim(new Claim(AuthConstants.CurrentUserResolvedClaimType, "true"));
        identity.AddClaims(result.Value.Permissions.Select(p => new Claim(AuthConstants.PermissionsClaimType, p.Name)));

        return new ClaimsPrincipal(identity);
    }
}
