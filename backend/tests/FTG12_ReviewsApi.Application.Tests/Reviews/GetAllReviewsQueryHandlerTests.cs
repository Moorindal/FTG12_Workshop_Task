using FluentAssertions;
using FTG12_ReviewsApi.Application.Reviews.Queries;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Reviews;

public class GetAllReviewsQueryHandlerTests
{
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly GetAllReviewsQueryHandler _handler;

    public GetAllReviewsQueryHandlerTests()
    {
        _handler = new GetAllReviewsQueryHandler(_reviewRepository);
    }

    [Fact]
    public async Task Handle_WithNoFilters_ReturnsAllReviews()
    {
        var reviews = CreateSampleReviews();
        _reviewRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetAllReviewsQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ReturnsMatchingReviews()
    {
        var reviews = CreateSampleReviews();
        _reviewRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetAllReviewsQuery(StatusId: 1), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items.Should().OnlyContain(r => r.StatusId == 1);
    }

    [Fact]
    public async Task Handle_WithDateFilter_ReturnsMatchingReviews()
    {
        var reviews = CreateSampleReviews();
        _reviewRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(reviews);

        var from = new DateTime(2026, 1, 2);
        var to = new DateTime(2026, 1, 2, 23, 59, 59);

        var result = await _handler.Handle(
            new GetAllReviewsQuery(DateFrom: from, DateTo: to), CancellationToken.None);

        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_WithCombinedFilters_ReturnsMatchingReviews()
    {
        var reviews = CreateSampleReviews();
        _reviewRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(
            new GetAllReviewsQuery(StatusId: 2, DateFrom: new DateTime(2026, 1, 1), DateTo: new DateTime(2026, 1, 2, 23, 59, 59)),
            CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].StatusId.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        var reviews = CreateSampleReviews();
        _reviewRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(
            new GetAllReviewsQuery(Page: 2, PageSize: 1), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(3);
    }

    private static List<Review> CreateSampleReviews()
    {
        return
        [
            new Review
            {
                Id = 1, ProductId = 1, UserId = 2, StatusId = 1, Rating = 3, Text = "Pending",
                CreatedAt = new DateTime(2026, 1, 1),
                Product = new Product { Id = 1, Name = "P1" },
                User = new User { Id = 2, Username = "User1" },
                Status = new ReviewStatus { Id = 1, Name = "Pending moderation" }
            },
            new Review
            {
                Id = 2, ProductId = 1, UserId = 3, StatusId = 2, Rating = 5, Text = "Approved",
                CreatedAt = new DateTime(2026, 1, 2),
                Product = new Product { Id = 1, Name = "P1" },
                User = new User { Id = 3, Username = "User2" },
                Status = new ReviewStatus { Id = 2, Name = "Approved" }
            },
            new Review
            {
                Id = 3, ProductId = 2, UserId = 2, StatusId = 2, Rating = 4, Text = "Also approved",
                CreatedAt = new DateTime(2026, 1, 3),
                Product = new Product { Id = 2, Name = "P2" },
                User = new User { Id = 2, Username = "User1" },
                Status = new ReviewStatus { Id = 2, Name = "Approved" }
            }
        ];
    }
}
