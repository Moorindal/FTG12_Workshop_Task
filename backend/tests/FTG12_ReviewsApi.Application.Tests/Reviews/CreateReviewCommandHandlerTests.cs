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

public class CreateReviewCommandHandlerTests
{
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly CreateReviewCommandHandler _handler;

    public CreateReviewCommandHandlerTests()
    {
        _handler = new CreateReviewCommandHandler(_currentUserService, _reviewRepository, _productRepository);
    }

    [Fact]
    public async Task Handle_WithValidInput_CreatesReviewWithPendingStatus()
    {
        _currentUserService.UserId.Returns(2);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Samsung Fridge" });
        _reviewRepository.GetByUserAndProductAsync(2, 1, Arg.Any<CancellationToken>())
            .Returns((Review?)null);

        var createdReview = new Review
        {
            Id = 10, ProductId = 1, UserId = 2, StatusId = 1, Rating = 4,
            Text = "Great product", CreatedAt = DateTime.UtcNow,
            Product = new Product { Id = 1, Name = "Samsung Fridge" },
            User = new User { Id = 2, Username = "User1" },
            Status = new ReviewStatus { Id = 1, Name = "Pending moderation" }
        };
        _reviewRepository.AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Review>());
        _reviewRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(createdReview);

        var result = await _handler.Handle(
            new CreateReviewCommand(1, 4, "Great product"), CancellationToken.None);

        result.StatusId.Should().Be(1);
        result.StatusName.Should().Be("Pending moderation");
        result.Rating.Should().Be(4);
        result.ProductName.Should().Be("Samsung Fridge");
    }

    [Fact]
    public async Task Handle_WithDuplicateReview_ThrowsConflict()
    {
        _currentUserService.UserId.Returns(2);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "P" });
        _reviewRepository.GetByUserAndProductAsync(2, 1, Arg.Any<CancellationToken>())
            .Returns(new Review { Id = 5 });

        Func<Task> act = () => _handler.Handle(
            new CreateReviewCommand(1, 4, "Text"), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ThrowsNotFound()
    {
        _currentUserService.UserId.Returns(2);
        _productRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Product?)null);

        Func<Task> act = () => _handler.Handle(
            new CreateReviewCommand(999, 4, "Text"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ThrowsUnauthorized()
    {
        _currentUserService.UserId.Returns((int?)null);

        Func<Task> act = () => _handler.Handle(
            new CreateReviewCommand(1, 4, "Text"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WhenBannedUser_BehaviorThrowsForbidden()
    {
        _currentUserService.UserId.Returns(2);
        var bannedUserRepo = Substitute.For<IBannedUserRepository>();
        bannedUserRepo.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var behavior = new BannedUserBehavior<CreateReviewCommand, ReviewDto>(
            _currentUserService, bannedUserRepo);
        var command = new CreateReviewCommand(1, 5, "Great");

        Func<Task> act = () => behavior.Handle(
            command,
            Substitute.For<RequestHandlerDelegate<ReviewDto>>(),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
