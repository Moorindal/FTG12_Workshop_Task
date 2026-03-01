using FTG12_ReviewsApi.Domain.Entities;

namespace FTG12_ReviewsApi.Application.Common.Interfaces;

/// <summary>
/// Generates JWT tokens for authenticated users.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT containing user claims (sub, unique_name, role).
    /// </summary>
    string GenerateToken(User user);
}
