using FluentAssertions;
using FTG12_ReviewsApi.Application.Common.Behaviors;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Reviews.Commands;
using FTG12_ReviewsApi.Application.Reviews.DTOs;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Reviews;

public class UpdateReviewCommandHandlerTests
{
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly UpdateReviewCommandHandler _handler;

    public UpdateReviewCommandHandlerTests()
    {
        _handler = new UpdateReviewCommandHandler(_currentUserService, _reviewRepository);
    }

    [Fact]
    public async Task Handle_WhenOwnerUpdates_ResetsStatusToPending()
    {
        _currentUserService.UserId.Returns(2);
        var review = CreateTestReview(10, 1, 2, statusId: 2);
        _reviewRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(review);

        var updatedReview = CreateTestReview(10, 1, 2, statusId: 1);
        updatedReview.Rating = 5;
        updatedReview.Text = "Updated text";
        updatedReview.Status = new ReviewStatus { Id = 1, Name = "Pending moderation" };
        _reviewRepository.When(r => r.UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>()))
            .Do(_ => { });
        _reviewRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(updatedReview);

        var result = await _handler.Handle(
            new UpdateReviewCommand(10, 5, "Updated text"), CancellationToken.None);

        result.StatusId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenNotOwner_ThrowsForbidden()
    {
        _currentUserService.UserId.Returns(3);
        var review = CreateTestReview(10, 1, 2);
        _reviewRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(review);

        Func<Task> act = () => _handler.Handle(
            new UpdateReviewCommand(10, 5, "Text"), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_WhenReviewNotFound_ThrowsNotFound()
    {
        _currentUserService.UserId.Returns(2);
        _reviewRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Review?)null);

        Func<Task> act = () => _handler.Handle(
            new UpdateReviewCommand(999, 5, "Text"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ThrowsUnauthorized()
    {
        _currentUserService.UserId.Returns((int?)null);

        Func<Task> act = () => _handler.Handle(
            new UpdateReviewCommand(1, 5, "Text"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WhenBannedUser_BehaviorThrowsForbidden()
    {
        _currentUserService.UserId.Returns(2);
        var bannedUserRepo = Substitute.For<IBannedUserRepository>();
        bannedUserRepo.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var behavior = new BannedUserBehavior<UpdateReviewCommand, ReviewDto>(
            _currentUserService, bannedUserRepo);

        Func<Task> act = () => behavior.Handle(
            new UpdateReviewCommand(10, 5, "Text"),
            Substitute.For<RequestHandlerDelegate<ReviewDto>>(),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    private static Review CreateTestReview(int id, int productId, int userId, int statusId = 1)
    {
        return new Review
        {
            Id = id, ProductId = productId, UserId = userId, StatusId = statusId,
            Rating = 4, Text = "Test review", CreatedAt = DateTime.UtcNow,
            Product = new Product { Id = productId, Name = "Product" },
            User = new User { Id = userId, Username = $"User{userId}" },
            Status = new ReviewStatus { Id = statusId, Name = statusId == 2 ? "Approved" : "Pending moderation" }
        };
    }
}
