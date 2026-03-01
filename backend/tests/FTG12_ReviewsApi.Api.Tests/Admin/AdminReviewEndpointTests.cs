using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FTG12_ReviewsApi.Api.Tests.Infrastructure;
using FTG12_ReviewsApi.Application.Common.Models;
using FTG12_ReviewsApi.Application.Reviews.DTOs;

namespace FTG12_ReviewsApi.Api.Tests.Admin;

public class AdminReviewEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AdminReviewEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllReviews_AsAdmin_Returns200()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        var response = await client.GetAsync("/api/admin/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<ReviewDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllReviews_AsNonAdmin_Returns403()
    {
        var client = _factory.CreateAuthenticatedClient(2, "User1", "User");

        var response = await client.GetAsync("/api/admin/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllReviews_WithStatusFilter_ReturnsFiltered()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        var response = await client.GetAsync("/api/admin/reviews?statusId=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<ReviewDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Items.Should().OnlyContain(r => r.StatusId == 2);
    }

    [Fact]
    public async Task ChangeReviewStatus_AsAdmin_Returns200()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        // Change review 4 (Pending) to Approved
        var response = await client.PutAsJsonAsync("/api/admin/reviews/4/status",
            new { StatusId = 2 });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ReviewDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.StatusId.Should().Be(2);
    }

    [Fact]
    public async Task ChangeReviewStatus_AsNonAdmin_Returns403()
    {
        var client = _factory.CreateAuthenticatedClient(2, "User1", "User");

        var response = await client.PutAsJsonAsync("/api/admin/reviews/1/status",
            new { StatusId = 2 });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ChangeReviewStatus_ReviewNotFound_Returns404()
    {
        var client = _factory.CreateAuthenticatedClient(1, "Admin", "Admin");

        var response = await client.PutAsJsonAsync("/api/admin/reviews/999/status",
            new { StatusId = 2 });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
