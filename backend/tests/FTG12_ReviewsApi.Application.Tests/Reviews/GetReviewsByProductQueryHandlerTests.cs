using FluentAssertions;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Reviews.Queries;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Reviews;

public class GetReviewsByProductQueryHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly GetReviewsByProductQueryHandler _handler;

    public GetReviewsByProductQueryHandlerTests()
    {
        _handler = new GetReviewsByProductQueryHandler(_productRepository, _reviewRepository, _currentUserService);
    }

    [Fact]
    public async Task Handle_WithApprovedReviews_ReturnsOnlyApproved()
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

        result.Reviews.TotalCount.Should().Be(2);
        result.Reviews.Items.Should().OnlyContain(r => r.StatusId == 2);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ThrowsNotFoundException()
    {
        _productRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Product?)null);

        Func<Task> act = () => _handler.Handle(
            new GetReviewsByProductQuery(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenNoApprovedReviews_ReturnsEmpty()
    {
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 1, "Pending", 3, "OK"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        result.Reviews.Items.Should().BeEmpty();
        result.Reviews.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = Enumerable.Range(1, 5)
            .Select(i => CreateReview(i, 1, i + 1, 2, "Approved", 4, $"Review {i}"))
            .ToList();
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(
            new GetReviewsByProductQuery(1, Page: 2, PageSize: 2), CancellationToken.None);

        result.Reviews.Items.Should().HaveCount(2);
        result.Reviews.TotalCount.Should().Be(5);
        result.Reviews.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WhenUserAuthenticated_ReturnsUserReview()
    {
        _currentUserService.UserId.Returns(3);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 2, "Approved", 5, "Great"),
            CreateReview(2, 1, 3, 1, "Pending moderation", 3, "OK"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        result.UserReview.Should().NotBeNull();
        result.UserReview!.UserId.Should().Be(3);
        result.UserReview.StatusId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ReturnsNullUserReview()
    {
        _currentUserService.UserId.Returns((int?)null);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 2, "Approved", 5, "Great"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        result.UserReview.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserHasNoReview_ReturnsNullUserReview()
    {
        _currentUserService.UserId.Returns(99);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 2, "Approved", 5, "Great"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        result.UserReview.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserAuthenticated_IncludesOwnPendingReviewInList()
    {
        _currentUserService.UserId.Returns(3);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 2, "Approved", 5, "Great"),
            CreateReview(2, 1, 3, 1, "Pending moderation", 3, "My pending review"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        result.Reviews.Items.Should().HaveCount(2);
        result.Reviews.Items.Should().Contain(r => r.UserId == 3 && r.StatusId == 1);
        result.Reviews.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenUserAuthenticated_IncludesOwnRejectedReviewInList()
    {
        _currentUserService.UserId.Returns(3);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 2, "Approved", 5, "Great"),
            CreateReview(2, 1, 3, 3, "Rejected", 2, "My rejected review"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        result.Reviews.Items.Should().HaveCount(2);
        result.Reviews.Items.Should().Contain(r => r.UserId == 3 && r.StatusId == 3);
        result.Reviews.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenUserAuthenticated_ExcludesOtherUsersPendingReviews()
    {
        _currentUserService.UserId.Returns(99);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 2, "Approved", 5, "Great"),
            CreateReview(2, 1, 3, 1, "Pending moderation", 3, "Other user pending"),
            CreateReview(3, 1, 4, 3, "Rejected", 2, "Other user rejected"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        result.Reviews.Items.Should().HaveCount(1);
        result.Reviews.Items.Should().OnlyContain(r => r.StatusId == 2);
        result.Reviews.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenUserAuthenticated_DoesNotDuplicateOwnApprovedReview()
    {
        _currentUserService.UserId.Returns(2);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 2, "Approved", 5, "My approved review"),
            CreateReview(2, 1, 3, 2, "Approved", 4, "Another review"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        result.Reviews.Items.Should().HaveCount(2);
        result.Reviews.Items.Count(r => r.UserId == 2).Should().Be(1);
        result.Reviews.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ReturnsOnlyApprovedReviews()
    {
        _currentUserService.UserId.Returns((int?)null);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = new List<Review>
        {
            CreateReview(1, 1, 2, 2, "Approved", 5, "Great"),
            CreateReview(2, 1, 3, 1, "Pending moderation", 3, "Pending"),
            CreateReview(3, 1, 4, 3, "Rejected", 2, "Rejected"),
        };
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetReviewsByProductQuery(1), CancellationToken.None);

        result.Reviews.Items.Should().HaveCount(1);
        result.Reviews.Items.Should().OnlyContain(r => r.StatusId == 2);
        result.Reviews.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_PaginationCountsIncludeUserOwnNonApprovedReviews()
    {
        _currentUserService.UserId.Returns(10);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Fridge" });

        var reviews = Enumerable.Range(1, 4)
            .Select(i => CreateReview(i, 1, i + 1, 2, "Approved", 4, $"Review {i}"))
            .Append(CreateReview(5, 1, 10, 1, "Pending moderation", 3, "My pending"))
            .ToList();
        _reviewRepository.GetByProductIdAsync(1, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(
            new GetReviewsByProductQuery(1, Page: 1, PageSize: 3), CancellationToken.None);

        result.Reviews.TotalCount.Should().Be(5);
        result.Reviews.Items.Should().HaveCount(3);
        result.Reviews.TotalPages.Should().Be(2);
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
