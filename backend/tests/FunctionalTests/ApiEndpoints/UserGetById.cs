using Web.Security.Users;
using Web.Security.Users.Get;

namespace FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class UserGetById(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ReturnsSeedUserGivenId1()
    {
        var result = await _client.GetAndDeserializeAsync<UserRecord>(GetUserRequest.BuildRoute(1));

        result.Id.ShouldBe(1);
        result.Name.ShouldBe("Seed User 1");
        result.Email.ShouldBe("user1@example.com");
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task ReturnsNotFoundGivenUnknownId()
    {
        string route = GetUserRequest.BuildRoute(9999);
        _ = await _client.GetAndEnsureNotFoundAsync(route);
    }
}
