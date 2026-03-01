using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FTG12_ReviewsApi.Api.Tests.Infrastructure;
using FTG12_ReviewsApi.Application.Common.Models;
using FTG12_ReviewsApi.Application.Reviews.DTOs;

namespace FTG12_ReviewsApi.Api.Tests.Reviews;

public class ReviewEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ReviewEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateReview_WithValidData_Returns201()
    {
        // User3 (id=4) has not reviewed product 1 in seed data
        var client = _factory.CreateAuthenticatedClient(4, "User3", "User");

        var response = await client.PostAsJsonAsync("/api/reviews",
            new { ProductId = 1, Rating = 4, Text = "Good product" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ReviewDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.StatusId.Should().Be(1);
        result.Rating.Should().Be(4);
    }

    [Fact]
    public async Task CreateReview_Duplicate_Returns409()
    {
        // User1 (id=2) already reviewed product 1 in seed data
        var client = _factory.CreateAuthenticatedClient(2, "User1", "User");

        var response = await client.PostAsJsonAsync("/api/reviews",
            new { ProductId = 1, Rating = 3, Text = "Duplicate" });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateReview_WithInvalidRating_Returns400()
    {
        var client = _factory.CreateAuthenticatedClient(4, "User3", "User");

        var response = await client.PostAsJsonAsync("/api/reviews",
            new { ProductId = 1, Rating = 6, Text = "Too high" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
    }

    [Fact]
    public async Task CreateReview_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/reviews",
            new { ProductId = 1, Rating = 4, Text = "Test" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateReview_ByOwner_Returns200()
    {
        // Review 1 belongs to User1 (id=2)
        var client = _factory.CreateAuthenticatedClient(2, "User1", "User");

        var response = await client.PutAsJsonAsync("/api/reviews/1",
            new { Rating = 3, Text = "Updated review text" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ReviewDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.StatusId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateReview_ByNonOwner_Returns403()
    {
        // Review 1 belongs to User1 (id=2), trying with User2 (id=3)
        var client = _factory.CreateAuthenticatedClient(3, "User2", "User");

        var response = await client.PutAsJsonAsync("/api/reviews/1",
            new { Rating = 3, Text = "Not my review" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateReview_NotFound_Returns404()
    {
        var client = _factory.CreateAuthenticatedClient(2, "User1", "User");

        var response = await client.PutAsJsonAsync("/api/reviews/999",
            new { Rating = 3, Text = "Missing" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReviewsByProduct_ReturnsApprovedOnly()
    {
        // Admin (id=1) has no reviews on product 1, so visible list should contain only approved
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/products/1/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductReviewsDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.Reviews.Items.Should().OnlyContain(r => r.StatusId == 2);
    }

    [Fact]
    public async Task GetReviewsByProduct_AuthUserWithPendingReview_IncludesInList()
    {
        // User3 (id=4) has a pending review for product 2 (StatusId=1)
        var client = _factory.CreateAuthenticatedClient(4, "User3", "User");

        var response = await client.GetAsync("/api/products/2/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductReviewsDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.Reviews.Items.Should().Contain(r => r.UserId == 4 && r.StatusId == 1);
    }

    [Fact]
    public async Task GetReviewsByProduct_AuthUserWithRejectedReview_IncludesInList()
    {
        // User3 (id=4) has a rejected review for product 3 (StatusId=3)
        var client = _factory.CreateAuthenticatedClient(4, "User3", "User");

        var response = await client.GetAsync("/api/products/3/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductReviewsDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.Reviews.Items.Should().Contain(r => r.UserId == 4 && r.StatusId == 3);
    }

    [Fact]
    public async Task GetReviewsByProduct_DifferentUser_ExcludesPendingReviews()
    {
        // User1 (id=2) viewing product 2 — should NOT see User3's pending review
        var client = _factory.CreateAuthenticatedClient(2, "User1", "User");

        var response = await client.GetAsync("/api/products/2/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductReviewsDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.Reviews.Items.Should().NotContain(r => r.UserId == 4 && r.StatusId == 1);
        result.Reviews.Items.Should().OnlyContain(r => r.StatusId == 2 || r.UserId == 2);
    }

    [Fact]
    public async Task GetReviewsByProduct_Unauthenticated_ReturnsOnlyApproved()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products/2/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductReviewsDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.Reviews.Items.Should().OnlyContain(r => r.StatusId == 2);
    }

    [Fact]
    public async Task GetReviewsByProduct_WhenUserHasReview_ReturnsUserReview()
    {
        // User1 (id=2) has an approved review for product 1
        var client = _factory.CreateAuthenticatedClient(2, "User1", "User");

        var response = await client.GetAsync("/api/products/1/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductReviewsDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.UserReview.Should().NotBeNull();
        result.UserReview!.UserId.Should().Be(2);
    }

    [Fact]
    public async Task GetReviewsByProduct_WhenUserHasNoReview_ReturnsNullUserReview()
    {
        // User3 (id=4) has no review for product 1
        var client = _factory.CreateAuthenticatedClient(4, "User3", "User");

        var response = await client.GetAsync("/api/products/1/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductReviewsDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.UserReview.Should().BeNull();
    }

    [Fact]
    public async Task GetMyReviews_ReturnsUserReviews()
    {
        // User1 (id=2) has reviews in seed data
        var client = _factory.CreateAuthenticatedClient(2, "User1", "User");

        var response = await client.GetAsync("/api/reviews/my");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<ReviewDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.Items.Should().OnlyContain(r => r.UserId == 2);
    }
}
