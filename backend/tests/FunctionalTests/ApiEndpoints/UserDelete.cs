using System.Net;
using Web.Security.Users.Delete;

namespace FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class UserDelete(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ReturnsNoContentWhenUserExists()
    {
        // Use the seed user (id=1)
        var response = await _client.DeleteAsync(
            DeleteUserRequest.BuildRoute(1),
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ReturnsNotFoundForUnknownId()
    {
        var response = await _client.DeleteAsync(
            DeleteUserRequest.BuildRoute(9999),
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
