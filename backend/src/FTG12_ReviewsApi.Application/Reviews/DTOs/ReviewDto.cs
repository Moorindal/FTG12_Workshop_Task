namespace FTG12_ReviewsApi.Application.Reviews.DTOs;

/// <summary>
/// DTO representing a review with related entity names.
/// </summary>
public record ReviewDto(
    int Id,
    int ProductId,
    string ProductName,
    int UserId,
    string Username,
    int StatusId,
    string StatusName,
    int Rating,
    string Text,
    DateTime CreatedAt);
