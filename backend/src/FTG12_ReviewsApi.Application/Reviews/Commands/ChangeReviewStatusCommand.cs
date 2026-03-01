using FluentValidation;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Reviews.DTOs;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Reviews.Commands;

/// <summary>
/// Command to change a review's moderation status (admin only).
/// </summary>
public record ChangeReviewStatusCommand(int Id, int StatusId) : IRequest<ReviewDto>;

/// <summary>
/// Validates <see cref="ChangeReviewStatusCommand"/> inputs.
/// </summary>
public class ChangeReviewStatusCommandValidator : AbstractValidator<ChangeReviewStatusCommand>
{
    public ChangeReviewStatusCommandValidator()
    {
        RuleFor(x => x.StatusId).Must(s => s is 2 or 3)
            .WithMessage("StatusId must be 2 (Approved) or 3 (Rejected).");
    }
}

/// <summary>
/// Handles <see cref="ChangeReviewStatusCommand"/> by updating the review's status.
/// </summary>
public class ChangeReviewStatusCommandHandler(IReviewRepository reviewRepository)
    : IRequestHandler<ChangeReviewStatusCommand, ReviewDto>
{
    public async Task<ReviewDto> Handle(ChangeReviewStatusCommand request, CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Review), request.Id);

        review.StatusId = request.StatusId;
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
