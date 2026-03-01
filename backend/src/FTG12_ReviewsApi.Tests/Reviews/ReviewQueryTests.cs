using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Reviews.Queries;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Tests.Reviews;

public class GetReviewsByProductQueryHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly GetReviewsByProductQueryHandler _handler;

    public GetReviewsByProductQueryHandlerTests()
    {
        _handler = new GetReviewsByProductQueryHandler(_productRepository, _reviewRepository);
    }

    [Fact]
    public async Task WhenProductExistsThenReturnsOnlyApprovedReviews()
    {
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 2, "Approved", 5, "Great"),
            CreateReview(2, 1, 3, 1, "Pending moderation", 3, "OK"),
            CreateReview(3, 1, 4, 2, "Approved", 4, "Good"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, r => Assert.Equal(2, r.StatusId));
    }

    [Fact]
    public async Task WhenProductNotFoundThenThrowsNotFoundException()
    {
        _productRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new GetReviewsByProductQuery(999), CancellationToken.None));
    }

    [Fact]
    public async Task WhenNoApprovedReviewsThenReturnsEmptyList()
    {
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 1, "Pending", 3, "OK"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    private static Review CreateReview(int id, int productId, int userId, int statusId, string statusName, int rating, string text)
    {
        return new Review
        {
            Id = id, ProductId = productId, UserId = userId, StatusId = statusId,
            Rating = rating, Text = text, CreatedAt = DateTime.UtcNow,
            Product = new Product { Id = productId, Name = "Product" },
            User = new User { Id = userId, Username = $"User{userId}" },
            Status = new ReviewStatus { Id = statusId, Name = statusName }
        };
    }
}

public class GetMyReviewsQueryHandlerTests
{
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly GetMyReviewsQueryHandler _handler;

    public GetMyReviewsQueryHandlerTests()
    {
        _handler = new GetMyReviewsQueryHandler(_currentUserService, _reviewRepository);
    }

    [Fact]
    public async Task WhenUserHasReviewsThenReturnsAllStatuses()
    {
        _currentUserService.UserId.Returns(2);

        var reviews = new List<Review>
        {
            new()
            {
                Id = 1, ProductId = 1, UserId = 2, StatusId = 1, Rating = 3, Text = "Pending review",
                CreatedAt = DateTime.UtcNow,
                Product = new Product { Id = 1, Name = "P1" },
                User = new User { Id = 2, Username = "User1" },
                Status = new ReviewStatus { Id = 1, Name = "Pending moderation" }
            },
            new()
            {
                Id = 2, ProductId = 2, UserId = 2, StatusId = 2, Rating = 5, Text = "Approved review",
                CreatedAt = DateTime.UtcNow,
                Product = new Product { Id = 2, Name = "P2" },
                User = new User { Id = 2, Username = "User1" },
                Status = new ReviewStatus { Id = 2, Name = "Approved" }
            },
        };
        _reviewRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetMyReviewsQuery(), CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Contains(result.Items, r => r.StatusId == 1);
        Assert.Contains(result.Items, r => r.StatusId == 2);
    }

    [Fact]
    public async Task WhenNotAuthenticatedThenThrowsUnauthorized()
    {
        _currentUserService.UserId.Returns((int?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(new GetMyReviewsQuery(), CancellationToken.None));
    }
}
