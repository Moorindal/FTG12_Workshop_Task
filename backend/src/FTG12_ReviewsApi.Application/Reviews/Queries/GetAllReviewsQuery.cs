using FluentValidation;
using FTG12_ReviewsApi.Application.Common.Models;
using FTG12_ReviewsApi.Application.Reviews.DTOs;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Reviews.Queries;

/// <summary>
/// Query to retrieve all reviews with optional filters (admin only).
/// </summary>
public record GetAllReviewsQuery(
    int? StatusId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int Page = 1,
    int PageSize = 10) : IRequest<PaginatedList<ReviewDto>>;

/// <summary>
/// Validates <see cref="GetAllReviewsQuery"/> inputs.
/// </summary>
public class GetAllReviewsQueryValidator : AbstractValidator<GetAllReviewsQuery>
{
    public GetAllReviewsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
        When(x => x.DateFrom.HasValue && x.DateTo.HasValue, () =>
        {
            RuleFor(x => x.DateFrom).LessThanOrEqualTo(x => x.DateTo)
                .WithMessage("dateFrom must be before or equal to dateTo.");
        });
    }
}

/// <summary>
/// Handles <see cref="GetAllReviewsQuery"/> by applying filters and pagination.
/// </summary>
public class GetAllReviewsQueryHandler(IReviewRepository reviewRepository)
    : IRequestHandler<GetAllReviewsQuery, PaginatedList<ReviewDto>>
{
    public async Task<PaginatedList<ReviewDto>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
    {
        var allReviews = await reviewRepository.GetAllAsync(cancellationToken);

        var filtered = allReviews.AsEnumerable();

        if (request.StatusId.HasValue)
        {
            filtered = filtered.Where(r => r.StatusId == request.StatusId.Value);
        }

        if (request.DateFrom.HasValue)
        {
            filtered = filtered.Where(r => r.CreatedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            filtered = filtered.Where(r => r.CreatedAt <= request.DateTo.Value);
        }

        var list = filtered.ToList();
        var totalCount = list.Count;

        var items = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReviewDto(
                r.Id, r.ProductId, r.Product.Name, r.UserId, r.User.Username,
                r.StatusId, r.Status.Name, r.Rating, r.Text, r.CreatedAt))
            .ToList();

        return PaginatedList<ReviewDto>.Create(items, request.Page, request.PageSize, totalCount);
    }
}
