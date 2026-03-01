namespace FTG12_ReviewsApi.Application.Common.Interfaces;

/// <summary>
/// Provides password hashing and verification capabilities.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes the specified plain-text password.
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Verifies a plain-text password against a previously hashed password.
    /// </summary>
    bool Verify(string password, string hash);
}
