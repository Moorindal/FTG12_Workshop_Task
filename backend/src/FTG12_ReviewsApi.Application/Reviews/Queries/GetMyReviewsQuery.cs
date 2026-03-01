using FluentValidation;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Common.Models;
using FTG12_ReviewsApi.Application.Reviews.DTOs;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Reviews.Queries;

/// <summary>
/// Query to retrieve all reviews by the current user across all statuses.
/// </summary>
public record GetMyReviewsQuery(int Page = 1, int PageSize = 10)
    : IRequest<PaginatedList<ReviewDto>>;

/// <summary>
/// Validates <see cref="GetMyReviewsQuery"/> inputs.
/// </summary>
public class GetMyReviewsQueryValidator : AbstractValidator<GetMyReviewsQuery>
{
    public GetMyReviewsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}

/// <summary>
/// Handles <see cref="GetMyReviewsQuery"/> by returning all of the current user's reviews.
/// </summary>
public class GetMyReviewsQueryHandler(
    ICurrentUserService currentUserService,
    IReviewRepository reviewRepository) : IRequestHandler<GetMyReviewsQuery, PaginatedList<ReviewDto>>
{
    public async Task<PaginatedList<ReviewDto>> Handle(GetMyReviewsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var username = currentUserService.Username ?? string.Empty;
        var allReviews = await reviewRepository.GetByUserIdAsync(userId, cancellationToken);
        var totalCount = allReviews.Count;

        var items = allReviews
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReviewDto(
                r.Id, r.ProductId, r.Product.Name, r.UserId, username,
                r.StatusId, r.Status.Name, r.Rating, r.Text, r.CreatedAt))
            .ToList();

        return PaginatedList<ReviewDto>.Create(items, request.Page, request.PageSize, totalCount);
    }
}
