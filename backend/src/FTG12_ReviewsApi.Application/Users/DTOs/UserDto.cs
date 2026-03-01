namespace FTG12_ReviewsApi.Application.Users.DTOs;

/// <summary>
/// DTO representing a user with ban status information.
/// </summary>
public record UserDto(
    int Id,
    string Username,
    bool IsAdministrator,
    bool IsBanned,
    DateTime? BannedAt,
    DateTime CreatedAt);
