namespace FTG12_ReviewsApi.Application.Common.Interfaces;

/// <summary>
/// Provides information about the currently authenticated user from the HTTP context.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// The current user's ID, or null if not authenticated.
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// The current user's username, or null if not authenticated.
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Whether the current user has administrator privileges.
    /// </summary>
    bool IsAdmin { get; }

    /// <summary>
    /// Whether the current request is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
