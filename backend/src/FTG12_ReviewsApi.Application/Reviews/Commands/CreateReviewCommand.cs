using FluentValidation;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Reviews.DTOs;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Reviews.Commands;

/// <summary>
/// Command to create a new review for a product.
/// </summary>
public record CreateReviewCommand(int ProductId, int Rating, string Text) : IRequest<ReviewDto>, IBannedUserCheck;

/// <summary>
/// Validates <see cref="CreateReviewCommand"/> inputs.
/// </summary>
public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Text).NotEmpty().MaximumLength(8000);
    }
}

/// <summary>
/// Handles <see cref="CreateReviewCommand"/> by creating a review with Pending status.
/// </summary>
public class CreateReviewCommandHandler(
    ICurrentUserService currentUserService,
    IReviewRepository reviewRepository,
    IProductRepository productRepository) : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.ProductId);

        var existing = await reviewRepository.GetByUserAndProductAsync(userId, request.ProductId, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException("You have already reviewed this product.");
        }

        var review = new Review
        {
            ProductId = request.ProductId,
            UserId = userId,
            StatusId = 1,
            Rating = request.Rating,
            Text = request.Text,
            CreatedAt = DateTime.UtcNow
        };

        await reviewRepository.AddAsync(review, cancellationToken);

        var created = await reviewRepository.GetByIdAsync(review.Id, cancellationToken);

        return new ReviewDto(
            created!.Id,
            created.ProductId,
            created.Product.Name,
            created.UserId,
            created.User.Username,
            created.StatusId,
            created.Status.Name,
            created.Rating,
            created.Text,
            created.CreatedAt);
    }
}
