using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Reviews.Commands;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Tests.Reviews;

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
    public async Task WhenValidInputThenCreatesReviewWithPendingStatus()
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
            .Returns(callInfo => callInfo.Arg<Review>());
        _reviewRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(createdReview);

        var result = await _handler.Handle(
            new CreateReviewCommand(1, 4, "Great product"), CancellationToken.None);

        Assert.Equal(1, result.StatusId);
        Assert.Equal("Pending moderation", result.StatusName);
        Assert.Equal(4, result.Rating);
    }

    [Fact]
    public async Task WhenProductNotFoundThenThrowsNotFoundException()
    {
        _currentUserService.UserId.Returns(2);
        _productRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new CreateReviewCommand(999, 4, "Text"), CancellationToken.None));
    }

    [Fact]
    public async Task WhenDuplicateReviewThenThrowsConflict()
    {
        _currentUserService.UserId.Returns(2);
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "P" });
        _reviewRepository.GetByUserAndProductAsync(2, 1, Arg.Any<CancellationToken>())
            .Returns(new Review { Id = 5 });

        await Assert.ThrowsAsync<ConflictException>(
            () => _handler.Handle(new CreateReviewCommand(1, 4, "Text"), CancellationToken.None));
    }

    [Fact]
    public async Task WhenNotAuthenticatedThenThrowsUnauthorized()
    {
        _currentUserService.UserId.Returns((int?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(new CreateReviewCommand(1, 4, "Text"), CancellationToken.None));
    }
}

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
    public async Task WhenOwnerUpdatesThenResetsStatusToPending()
    {
        _currentUserService.UserId.Returns(2);
        var review = new Review
        {
            Id = 10, ProductId = 1, UserId = 2, StatusId = 2, Rating = 4,
            Text = "Old text", CreatedAt = DateTime.UtcNow,
            Product = new Product { Id = 1, Name = "P" },
            User = new User { Id = 2, Username = "User1" },
            Status = new ReviewStatus { Id = 2, Name = "Approved" }
        };
        _reviewRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(review);

        var updatedReview = new Review
        {
            Id = 10, ProductId = 1, UserId = 2, StatusId = 1, Rating = 5,
            Text = "Updated text", CreatedAt = review.CreatedAt,
            Product = new Product { Id = 1, Name = "P" },
            User = new User { Id = 2, Username = "User1" },
            Status = new ReviewStatus { Id = 1, Name = "Pending moderation" }
        };

        _reviewRepository.When(x => x.UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>()))
            .Do(_ => { });
        _reviewRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(updatedReview);

        var result = await _handler.Handle(
            new UpdateReviewCommand(10, 5, "Updated text"), CancellationToken.None);

        Assert.Equal(1, result.StatusId);
    }

    [Fact]
    public async Task WhenNotOwnerThenThrowsForbidden()
    {
        _currentUserService.UserId.Returns(3);
        var review = new Review
        {
            Id = 10, UserId = 2,
            Product = new Product { Id = 1, Name = "P" },
            User = new User { Id = 2, Username = "User1" },
            Status = new ReviewStatus { Id = 1, Name = "Pending" }
        };
        _reviewRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(review);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _handler.Handle(new UpdateReviewCommand(10, 5, "Text"), CancellationToken.None));
    }

    [Fact]
    public async Task WhenReviewNotFoundThenThrowsNotFound()
    {
        _currentUserService.UserId.Returns(2);
        _reviewRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Review?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new UpdateReviewCommand(999, 5, "Text"), CancellationToken.None));
    }
}

public class ReviewCommandValidatorTests
{
    [Fact]
    public void CreateReviewValidator_WhenRatingOutOfRangeThenFails()
    {
        var validator = new CreateReviewCommandValidator();
        var result = validator.Validate(new CreateReviewCommand(1, 6, "Text"));
        Assert.Contains(result.Errors, e => e.PropertyName == "Rating");
    }

    [Fact]
    public void CreateReviewValidator_WhenTextEmptyThenFails()
    {
        var validator = new CreateReviewCommandValidator();
        var result = validator.Validate(new CreateReviewCommand(1, 3, ""));
        Assert.Contains(result.Errors, e => e.PropertyName == "Text");
    }

    [Fact]
    public void CreateReviewValidator_WhenTextTooLongThenFails()
    {
        var validator = new CreateReviewCommandValidator();
        var result = validator.Validate(new CreateReviewCommand(1, 3, new string('x', 8001)));
        Assert.Contains(result.Errors, e => e.PropertyName == "Text");
    }

    [Fact]
    public void CreateReviewValidator_WhenValidThenPasses()
    {
        var validator = new CreateReviewCommandValidator();
        var result = validator.Validate(new CreateReviewCommand(1, 3, "Good product"));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateReviewValidator_WhenRatingZeroThenFails()
    {
        var validator = new UpdateReviewCommandValidator();
        var result = validator.Validate(new UpdateReviewCommand(1, 0, "Text"));
        Assert.Contains(result.Errors, e => e.PropertyName == "Rating");
    }

    [Fact]
    public void UpdateReviewValidator_WhenValidThenPasses()
    {
        var validator = new UpdateReviewCommandValidator();
        var result = validator.Validate(new UpdateReviewCommand(1, 5, "Good"));
        Assert.True(result.IsValid);
    }
}
