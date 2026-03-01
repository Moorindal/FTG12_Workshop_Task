using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FTG12_ReviewsApi.Api.Tests.Infrastructure;
using FTG12_ReviewsApi.Application.Users.DTOs;

namespace FTG12_ReviewsApi.Api.Tests.Admin;

public class AdminUserEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AdminUserEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUsers_AsAdmin_Returns200WithUsers()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        var response = await client.GetAsync("/api/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<UserDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetUsers_AsNonAdmin_Returns403()
    {
        var client = _factory.CreateAuthenticatedClient(2, "User1", "User");

        var response = await client.GetAsync("/api/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task BanUser_AsAdmin_Returns200()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        var response = await client.PostAsync("/api/admin/users/2/ban", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.IsBanned.Should().BeTrue();
    }

    [Fact]
    public async Task BanUser_AlreadyBanned_Returns409()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        // Ban user 3 first
        await client.PostAsync("/api/admin/users/3/ban", null);
        // Try banning again
        var response = await client.PostAsync("/api/admin/users/3/ban", null);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task BanUser_UserNotFound_Returns404()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        var response = await client.PostAsync("/api/admin/users/999/ban", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UnbanUser_AsAdmin_Returns200()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        // Ban user 4 first, then unban
        await client.PostAsync("/api/admin/users/4/ban", null);
        var response = await client.PostAsync("/api/admin/users/4/unban", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.IsBanned.Should().BeFalse();
    }

    [Fact]
    public async Task UnbanUser_NotBanned_Returns404()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        // User 2 is not banned initially (unless a previous test banned them, but each class gets its own DB)
        // Use a user that's definitely not banned - create fresh factory context
        var response = await client.PostAsync("/api/admin/users/1/unban", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
