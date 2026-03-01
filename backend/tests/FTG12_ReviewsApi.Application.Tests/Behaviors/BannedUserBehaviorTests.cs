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

namespace FTG12_ReviewsApi.Application.Tests.Behaviors;

public class BannedUserBehaviorTests
{
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IBannedUserRepository _bannedUserRepository = Substitute.For<IBannedUserRepository>();

    [Fact]
    public async Task Handle_WhenUserIsBanned_ThrowsForbidden()
    {
        _currentUserService.UserId.Returns(2);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var behavior = new BannedUserBehavior<CreateReviewCommand, ReviewDto>(
            _currentUserService, _bannedUserRepository);
        var command = new CreateReviewCommand(1, 5, "Great");

        Func<Task> act = () => behavior.Handle(
            command,
            Substitute.For<RequestHandlerDelegate<ReviewDto>>(),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("*banned*");
    }

    [Fact]
    public async Task Handle_WhenUserIsNotBanned_ProceedsToNext()
    {
        _currentUserService.UserId.Returns(2);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns((BannedUser?)null);

        var behavior = new BannedUserBehavior<CreateReviewCommand, ReviewDto>(
            _currentUserService, _bannedUserRepository);
        var expectedDto = new ReviewDto(1, 1, "Product", 2, "User", 1, "Pending", 5, "Great", DateTime.UtcNow);
        var next = Substitute.For<RequestHandlerDelegate<ReviewDto>>();
        next.Invoke(Arg.Any<CancellationToken>()).Returns(expectedDto);

        var result = await behavior.Handle(
            new CreateReviewCommand(1, 5, "Great"), next, CancellationToken.None);

        result.Should().Be(expectedDto);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ProceedsToNext()
    {
        _currentUserService.UserId.Returns((int?)null);

        var behavior = new BannedUserBehavior<CreateReviewCommand, ReviewDto>(
            _currentUserService, _bannedUserRepository);
        var expectedDto = new ReviewDto(1, 1, "Product", 2, "User", 1, "Pending", 5, "Great", DateTime.UtcNow);
        var next = Substitute.For<RequestHandlerDelegate<ReviewDto>>();
        next.Invoke(Arg.Any<CancellationToken>()).Returns(expectedDto);

        var result = await behavior.Handle(
            new CreateReviewCommand(1, 5, "Great"), next, CancellationToken.None);

        result.Should().Be(expectedDto);
    }

    [Fact]
    public async Task Handle_WithUpdateReviewCommand_WhenBanned_ThrowsForbidden()
    {
        _currentUserService.UserId.Returns(2);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var behavior = new BannedUserBehavior<UpdateReviewCommand, ReviewDto>(
            _currentUserService, _bannedUserRepository);

        Func<Task> act = () => behavior.Handle(
            new UpdateReviewCommand(1, 5, "Text"),
            Substitute.For<RequestHandlerDelegate<ReviewDto>>(),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
