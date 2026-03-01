using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FTG12_ReviewsApi.Api.Tests.Infrastructure;
using FTG12_ReviewsApi.Application.Auth.DTOs;

namespace FTG12_ReviewsApi.Api.Tests.Auth;

public class AuthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AuthEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { Username = "Admin", Password = "Admin" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        result.Should().NotBeNull();
        result!.Token.Should().NotBeEmpty();
        result.User.Username.Should().Be("Admin");
        result.User.IsAdministrator.Should().BeTrue();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { Username = "Admin", Password = "wrong" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithMissingFields_Returns400()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { Username = "", Password = "" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
    }

    [Fact]
    public async Task GetCurrentUser_WithToken_Returns200()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        var response = await client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserInfoDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.Username.Should().Be("Admin");
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithToken_Returns200()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PostAsync("/api/auth/logout", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
