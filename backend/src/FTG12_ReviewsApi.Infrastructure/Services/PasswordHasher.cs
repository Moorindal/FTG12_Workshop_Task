using FTG12_ReviewsApi.Application.Common.Interfaces;

namespace FTG12_ReviewsApi.Infrastructure.Services;

/// <summary>
/// BCrypt-based password hashing implementation.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
