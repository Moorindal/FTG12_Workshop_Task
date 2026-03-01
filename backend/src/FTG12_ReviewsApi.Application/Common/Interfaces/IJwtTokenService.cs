using FTG12_ReviewsApi.Domain.Entities;

namespace FTG12_ReviewsApi.Application.Common.Interfaces;

/// <summary>
/// Generates JWT tokens for authenticated users.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT containing user claims based on <see cref="System.Security.Claims.ClaimTypes.NameIdentifier"/>,
    /// <see cref="System.Security.Claims.ClaimTypes.Name"/>, and <see cref="System.Security.Claims.ClaimTypes.Role"/>.
    /// </summary>
    string GenerateToken(User user);
}
