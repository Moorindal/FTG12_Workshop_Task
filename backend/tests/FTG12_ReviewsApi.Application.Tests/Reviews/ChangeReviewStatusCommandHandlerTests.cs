using FluentAssertions;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Reviews.Commands;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Reviews;

public class ChangeReviewStatusCommandHandlerTests
{
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly ChangeReviewStatusCommandHandler _handler;

    public ChangeReviewStatusCommandHandlerTests()
    {
        _handler = new ChangeReviewStatusCommandHandler(_reviewRepository);
    }

    [Fact]
    public async Task Handle_WithValidStatus_ChangesReviewStatus()
    {
        var review = new Review
        {
            Id = 1, ProductId = 1, UserId = 2, StatusId = 1, Rating = 4, Text = "Good",
            CreatedAt = DateTime.UtcNow,
            Product = new Product { Id = 1, Name = "P" },
            User = new User { Id = 2, Username = "User1" },
            Status = new ReviewStatus { Id = 1, Name = "Pending moderation" }
        };
        _reviewRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(review);

        var updatedReview = new Review
        {
            Id = 1, ProductId = 1, UserId = 2, StatusId = 2, Rating = 4, Text = "Good",
            CreatedAt = review.CreatedAt,
            Product = new Product { Id = 1, Name = "P" },
            User = new User { Id = 2, Username = "User1" },
            Status = new ReviewStatus { Id = 2, Name = "Approved" }
        };
        _reviewRepository.When(r => r.UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>()))
            .Do(_ => { });
        _reviewRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(updatedReview);

        var result = await _handler.Handle(new ChangeReviewStatusCommand(1, 2), CancellationToken.None);

        result.StatusId.Should().Be(2);
        result.StatusName.Should().Be("Approved");
    }

    [Fact]
    public async Task Handle_WhenReviewNotFound_ThrowsNotFound()
    {
        _reviewRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Review?)null);

        Func<Task> act = () => _handler.Handle(
            new ChangeReviewStatusCommand(999, 2), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public async Task Handle_WithApprovedOrRejected_UpdatesSuccessfully(int statusId)
    {
        var statusName = statusId == 2 ? "Approved" : "Rejected";
        var review = new Review
        {
            Id = 1, ProductId = 1, UserId = 2, StatusId = 1, Rating = 4, Text = "Good",
            CreatedAt = DateTime.UtcNow,
            Product = new Product { Id = 1, Name = "P" },
            User = new User { Id = 2, Username = "User1" },
            Status = new ReviewStatus { Id = 1, Name = "Pending moderation" }
        };
        _reviewRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(review);

        var updatedReview = new Review
        {
            Id = 1, ProductId = 1, UserId = 2, StatusId = statusId, Rating = 4, Text = "Good",
            CreatedAt = review.CreatedAt,
            Product = new Product { Id = 1, Name = "P" },
            User = new User { Id = 2, Username = "User1" },
            Status = new ReviewStatus { Id = statusId, Name = statusName }
        };
        _reviewRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(updatedReview);

        var result = await _handler.Handle(new ChangeReviewStatusCommand(1, statusId), CancellationToken.None);

        result.StatusId.Should().Be(statusId);
        result.StatusName.Should().Be(statusName);
    }
}
