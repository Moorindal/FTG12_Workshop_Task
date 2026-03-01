using FluentValidation;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Common.Models;
using FTG12_ReviewsApi.Application.Reviews.DTOs;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Reviews.Queries;

/// <summary>
/// Query to retrieve paginated approved reviews for a specific product,
/// along with the current user's review (any status) if one exists.
/// </summary>
public record GetReviewsByProductQuery(int ProductId, int Page = 1, int PageSize = 10)
    : IRequest<ProductReviewsDto>;

/// <summary>
/// Validates <see cref="GetReviewsByProductQuery"/> inputs.
/// </summary>
public class GetReviewsByProductQueryValidator : AbstractValidator<GetReviewsByProductQuery>
{
    public GetReviewsByProductQueryValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}

/// <summary>
/// Handles <see cref="GetReviewsByProductQuery"/> by returning approved reviews
/// from all users plus the authenticated user's own reviews (any status),
/// and the authenticated user's own review as a separate field.
/// </summary>
public class GetReviewsByProductQueryHandler(
    IProductRepository productRepository,
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetReviewsByProductQuery, ProductReviewsDto>
{
    public async Task<ProductReviewsDto> Handle(GetReviewsByProductQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.ProductId);

        var allReviews = await reviewRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        int? currentUserId = currentUserService.UserId;

        var visibleReviews = allReviews
            .Where(r => r.StatusId == 2 || (currentUserId.HasValue && r.UserId == currentUserId.Value))
            .ToList();

        var totalCount = visibleReviews.Count;

        var items = visibleReviews
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReviewDto(
                r.Id, r.ProductId, product.Name, r.UserId, r.User.Username,
                r.StatusId, r.Status.Name, r.Rating, r.Text, r.CreatedAt))
            .ToList();

        var paginatedReviews = PaginatedList<ReviewDto>.Create(items, request.Page, request.PageSize, totalCount);

        ReviewDto? userReview = null;
        if (currentUserId is { } userId)
        {
            var ownReview = allReviews.FirstOrDefault(r => r.UserId == userId);
            if (ownReview is not null)
            {
                userReview = new ReviewDto(
                    ownReview.Id, ownReview.ProductId, product.Name, ownReview.UserId, ownReview.User.Username,
                    ownReview.StatusId, ownReview.Status.Name, ownReview.Rating, ownReview.Text, ownReview.CreatedAt);
            }
        }

        return new ProductReviewsDto(paginatedReviews, userReview);
    }
}
