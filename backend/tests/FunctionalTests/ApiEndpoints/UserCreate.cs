using System.Net;
using System.Net.Http.Json;
using Web.Security.Users.Create;
using Web.Security.Users.Get;

namespace FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class UserCreate(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ReturnsCreatedWithLocationHeader()
    {
        var request = new CreateUserRequest
        {
            Name = "New User",
            Email = "newuser@example.com",
            ExternalIdentifier = "ext-new"
        };

        var ct = TestContext.Current.CancellationToken;
        var response = await _client.PostAsJsonAsync(CreateUserRequest.Route, request, ct);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();

        var body = await response.Content.ReadFromJsonAsync<CreateUserResponse>(ct);
        body.ShouldNotBeNull();
        body.Id.ShouldBeGreaterThan(0);

        // verify the Location header resolves to the created user
        string expectedRoute = GetUserRequest.BuildRoute(body.Id);
        response.Headers.Location!.ToString().ShouldContain(expectedRoute);
    }

    [Fact]
    public async Task ReturnsBadRequestWhenNameIsMissing()
    {
        var request = new CreateUserRequest
        {
            Name = "",
            Email = "test@example.com",
            ExternalIdentifier = "ext-x"
        };

        var response = await _client.PostAsJsonAsync(CreateUserRequest.Route, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReturnsBadRequestWhenEmailIsInvalid()
    {
        var request = new CreateUserRequest
        {
            Name = "Test",
            Email = "not-an-email",
            ExternalIdentifier = "ext-x"
        };

        var response = await _client.PostAsJsonAsync(CreateUserRequest.Route, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
