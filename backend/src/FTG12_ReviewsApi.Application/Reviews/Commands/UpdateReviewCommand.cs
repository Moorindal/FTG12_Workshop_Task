using FluentValidation;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Reviews.DTOs;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Reviews.Commands;

/// <summary>
/// Command to update an existing review owned by the current user.
/// </summary>
public record UpdateReviewCommand(int Id, int Rating, string Text) : IRequest<ReviewDto>, IBannedUserCheck;

/// <summary>
/// Validates <see cref="UpdateReviewCommand"/> inputs.
/// </summary>
public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Text).NotEmpty().MaximumLength(8000);
    }
}

/// <summary>
/// Handles <see cref="UpdateReviewCommand"/> by verifying ownership and resetting status to Pending.
/// </summary>
public class UpdateReviewCommandHandler(
    ICurrentUserService currentUserService,
    IReviewRepository reviewRepository) : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (currentUserService.IsAdmin)
        {
            throw new ForbiddenException("Administrators cannot edit reviews.");
        }

        var review = await reviewRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Review), request.Id);

        if (review.UserId != userId)
        {
            throw new ForbiddenException("You can only update your own reviews.");
        }

        review.Rating = request.Rating;
        review.Text = request.Text;
        review.StatusId = 1;

        await reviewRepository.UpdateAsync(review, cancellationToken);

        var updated = await reviewRepository.GetByIdAsync(review.Id, cancellationToken);

        return new ReviewDto(
            updated!.Id,
            updated.ProductId,
            updated.Product.Name,
            updated.UserId,
            updated.User.Username,
            updated.StatusId,
            updated.Status.Name,
            updated.Rating,
            updated.Text,
            updated.CreatedAt);
    }
}
