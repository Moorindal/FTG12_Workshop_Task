namespace FTG12_ReviewsApi.Application.Auth.DTOs;

/// <summary>
/// DTO containing user identity information.
/// </summary>
public record UserInfoDto(int Id, string Username, bool IsAdministrator, bool IsBanned);
