using System.Net;
using System.Net.Http.Json;
using Web.Security.Users.Update;

namespace FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class UserUpdate(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ReturnsUpdatedUserWhenExists()
    {
        var request = new UpdateUserRequest
        {
            Id = 1,
            Name = "Updated Name",
            Email = "updated@example.com",
            IsActive = false
        };

        var ct = TestContext.Current.CancellationToken;
        var response = await _client.PutAsJsonAsync(UpdateUserRequest.BuildRoute(1), request, ct);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ReturnsNotFoundForUnknownId()
    {
        var request = new UpdateUserRequest
        {
            Id = 9999,
            Name = "Name",
            Email = "email@example.com",
            IsActive = true
        };

        var response = await _client.PutAsJsonAsync(
            UpdateUserRequest.BuildRoute(9999), request,
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReturnsBadRequestWhenEmailIsInvalid()
    {
        var request = new UpdateUserRequest
        {
            Id = 1,
            Name = "Name",
            Email = "not-an-email",
            IsActive = true
        };

        var response = await _client.PutAsJsonAsync(
            UpdateUserRequest.BuildRoute(1), request,
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
