namespace FTG12_ReviewsApi.Application.Auth.DTOs;

/// <summary>
/// Response DTO for a successful login.
/// </summary>
public record LoginResponse(string Token, UserInfoDto User);
