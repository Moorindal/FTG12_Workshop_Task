using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Reviews.Commands;
using FTG12_ReviewsApi.Application.Reviews.Queries;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Tests.Reviews;

public class GetAllReviewsQueryHandlerTests
{
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly GetAllReviewsQueryHandler _handler;

    public GetAllReviewsQueryHandlerTests()
    {
        _handler = new GetAllReviewsQueryHandler(_reviewRepository);
    }

    [Fact]
    public async Task WhenNoFiltersThenReturnsAllReviews()
    {
        var reviews = CreateSampleReviews();
        _reviewRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetAllReviewsQuery(), CancellationToken.None);

        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task WhenFilterByStatusThenReturnsMatchingReviews()
    {
        var reviews = CreateSampleReviews();
        _reviewRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(reviews);

        var result = await _handler.Handle(new GetAllReviewsQuery(StatusId: 1), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.All(result.Items, r => Assert.Equal(1, r.StatusId));
    }

    [Fact]
    public async Task WhenFilterByDateRangeThenReturnsMatchingReviews()
    {
        var reviews = CreateSampleReviews();
        _reviewRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(reviews);

        var from = new DateTime(2026, 1, 2);
        var to = new DateTime(2026, 1, 2, 23, 59, 59);

        var result = await _handler.Handle(new GetAllReviewsQuery(DateFrom: from, DateTo: to), CancellationToken.None);

        Assert.Single(result.Items);
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

public class ChangeReviewStatusCommandHandlerTests
{
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly ChangeReviewStatusCommandHandler _handler;

    public ChangeReviewStatusCommandHandlerTests()
    {
        _handler = new ChangeReviewStatusCommandHandler(_reviewRepository);
    }

    [Fact]
    public async Task WhenReviewExistsThenChangesStatus()
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

        _reviewRepository.When(x => x.UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>()))
            .Do(_ => { });

        _reviewRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(updatedReview);

        var result = await _handler.Handle(new ChangeReviewStatusCommand(1, 2), CancellationToken.None);

        Assert.Equal(2, result.StatusId);
    }

    [Fact]
    public async Task WhenReviewNotFoundThenThrowsNotFound()
    {
        _reviewRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Review?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new ChangeReviewStatusCommand(999, 2), CancellationToken.None));
    }
}

public class ChangeReviewStatusCommandValidatorTests
{
    private readonly ChangeReviewStatusCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    public void WhenInvalidStatusIdThenFails(int statusId)
    {
        var result = _validator.Validate(new ChangeReviewStatusCommand(1, statusId));
        Assert.Contains(result.Errors, e => e.PropertyName == "StatusId");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public void WhenValidStatusIdThenPasses(int statusId)
    {
        var result = _validator.Validate(new ChangeReviewStatusCommand(1, statusId));
        Assert.True(result.IsValid);
    }
}
