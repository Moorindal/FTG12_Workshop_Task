using FluentAssertions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Reviews.Queries;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Reviews;

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
    public async Task Handle_WhenUserHasReviews_ReturnsAllStatuses()
    {
        _currentUserService.UserId.Returns(2);
        _currentUserService.Username.Returns("User1");

        var reviews = new List<Review>
        {
            new()
            {
                Id = 1, ProductId = 1, UserId = 2, StatusId = 1, Rating = 3, Text = "Pending review",
                CreatedAt = DateTime.UtcNow,
                Product = new Product { Id = 1, Name = "P1" },
                Status = new ReviewStatus { Id = 1, Name = "Pending moderation" }
            },
            new()
            {
                Id = 2, ProductId = 2, UserId = 2, StatusId = 2, Rating = 5, Text = "Approved review",
                CreatedAt = DateTime.UtcNow,
                Product = new Product { Id = 2, Name = "P2" },
                Status = new ReviewStatus { Id = 2, Name = "Approved" }
            },
        };
        _reviewRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetMyReviewsQuery(), CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Items.Should().Contain(r => r.StatusId == 1);
        result.Items.Should().Contain(r => r.StatusId == 2);
        result.Items.Should().OnlyContain(r => r.Username == "User1");
    }

    [Fact]
    public async Task Handle_WhenNoReviews_ReturnsEmpty()
    {
        _currentUserService.UserId.Returns(2);
        _currentUserService.Username.Returns("User1");
        _reviewRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>()).Returns(new List<Review>());

        var result = await _handler.Handle(new GetMyReviewsQuery(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ThrowsUnauthorized()
    {
        _currentUserService.UserId.Returns((int?)null);

        Func<Task> act = () => _handler.Handle(new GetMyReviewsQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
