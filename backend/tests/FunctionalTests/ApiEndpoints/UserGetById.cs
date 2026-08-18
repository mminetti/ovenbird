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
        var result = await _client.GetAndDeserializeAsync<UserRecord>(GetUserByIdRequest.BuildRoute(1));

        result.Id.ShouldBe(1);
        result.Name.ShouldBe("Seed User 1");
        result.Email.ShouldBe("user1@example.com");
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task ReturnsNotFoundGivenUnknownId()
    {
        string route = GetUserByIdRequest.BuildRoute(9999);
        _ = await _client.GetAndEnsureNotFoundAsync(route);
    }
}
