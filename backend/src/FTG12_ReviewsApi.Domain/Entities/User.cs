namespace FTG12_ReviewsApi.Domain.Entities;

/// <summary>
/// Represents an application user.
/// </summary>
public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsAdministrator { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Review> Reviews { get; set; } = [];
}
