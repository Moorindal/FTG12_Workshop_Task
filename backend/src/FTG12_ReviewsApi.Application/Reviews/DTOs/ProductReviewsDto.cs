using FTG12_ReviewsApi.Application.Common.Models;

namespace FTG12_ReviewsApi.Application.Reviews.DTOs;

/// <summary>
/// Response DTO for product reviews, including the current user's review if one exists.
/// </summary>
public record ProductReviewsDto(PaginatedList<ReviewDto> Reviews, ReviewDto? UserReview);
