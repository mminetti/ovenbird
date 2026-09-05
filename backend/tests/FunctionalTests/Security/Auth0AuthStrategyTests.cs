using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Web.Configurations.Auth;

namespace FunctionalTests.Security;

public class Auth0AuthStrategyTests
{
    private const string Namespace = "https://test.example.com/";

    private static Auth0AuthStrategy CreateStrategy(string? claimsNamespace = Namespace)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth0:ClaimsNamespace"] = claimsNamespace
            })
            .Build();

        return new Auth0AuthStrategy(configuration);
    }

    private static ClaimsPrincipal PrincipalWithClaims(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, "Bearer"));

    [Fact]
    public void GetExternalIdentifier_ReturnsSubClaim()
    {
        var strategy = CreateStrategy();
        var principal = PrincipalWithClaims(new Claim("sub", "auth0|abc123"));

        strategy.GetExternalIdentifier(principal).ShouldBe("auth0|abc123");
    }

    [Fact]
    public void GetExternalIdentifier_ReturnsNull_WhenSubClaimAbsent()
    {
        var strategy = CreateStrategy();
        var principal = PrincipalWithClaims();

        strategy.GetExternalIdentifier(principal).ShouldBeNull();
    }

    [Fact]
    public void GetName_ReturnsNamespacedClaim()
    {
        var strategy = CreateStrategy();
        var principal = PrincipalWithClaims(new Claim($"{Namespace}name", "Ada Lovelace"));

        strategy.GetName(principal).ShouldBe("Ada Lovelace");
    }

    [Fact]
    public void GetName_ReturnsEmpty_WhenNamespacedClaimAbsent()
    {
        var strategy = CreateStrategy();
        var principal = PrincipalWithClaims();

        strategy.GetName(principal).ShouldBe(string.Empty);
    }

    [Fact]
    public void GetEmail_ReturnsNamespacedClaim()
    {
        var strategy = CreateStrategy();
        var principal = PrincipalWithClaims(new Claim($"{Namespace}email", "ada@example.com"));

        strategy.GetEmail(principal).ShouldBe("ada@example.com");
    }

    [Fact]
    public void GetEmail_ReturnsEmpty_WhenNamespacedClaimAbsent()
    {
        var strategy = CreateStrategy();
        var principal = PrincipalWithClaims();

        strategy.GetEmail(principal).ShouldBe(string.Empty);
    }

    [Fact]
    public void GetName_DoesNotMatch_WhenNamespaceTrailingSlashDiffers()
    {
        var strategy = CreateStrategy();
        // Claim uses no trailing slash before "name", so concatenation with the configured
        // namespace (which has one) must not accidentally match.
        var principal = PrincipalWithClaims(new Claim("https://test.example.comname", "Ada Lovelace"));

        strategy.GetName(principal).ShouldBe(string.Empty);
    }

    [Fact]
    public void UsesDefaultNamespace_WhenNotConfigured()
    {
        var strategy = CreateStrategy(claimsNamespace: null);
        var principal = PrincipalWithClaims(new Claim("https://ovenbird.example.com/name", "Ada Lovelace"));

        strategy.GetName(principal).ShouldBe("Ada Lovelace");
    }
}
