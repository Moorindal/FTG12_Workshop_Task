using FluentValidation;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Models;
using FTG12_ReviewsApi.Application.Reviews.DTOs;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Reviews.Queries;

/// <summary>
/// Query to retrieve paginated approved reviews for a specific product.
/// </summary>
public record GetReviewsByProductQuery(int ProductId, int Page = 1, int PageSize = 10)
    : IRequest<PaginatedList<ReviewDto>>;

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
/// Handles <see cref="GetReviewsByProductQuery"/> by returning only approved reviews.
/// </summary>
public class GetReviewsByProductQueryHandler(
    IProductRepository productRepository,
    IReviewRepository reviewRepository) : IRequestHandler<GetReviewsByProductQuery, PaginatedList<ReviewDto>>
{
    public async Task<PaginatedList<ReviewDto>> Handle(GetReviewsByProductQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.ProductId);

        var allReviews = await reviewRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        var approvedReviews = allReviews
            .Where(r => r.StatusId == 2)
            .ToList();

        var totalCount = approvedReviews.Count;

        var items = approvedReviews
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReviewDto(
                r.Id, r.ProductId, product.Name, r.UserId, r.User.Username,
                r.StatusId, r.Status.Name, r.Rating, r.Text, r.CreatedAt))
            .ToList();

        return PaginatedList<ReviewDto>.Create(items, request.Page, request.PageSize, totalCount);
    }
}
