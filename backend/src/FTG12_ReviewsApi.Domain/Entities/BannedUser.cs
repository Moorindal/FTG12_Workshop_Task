namespace FTG12_ReviewsApi.Domain.Entities;

/// <summary>
/// Represents a banned user record.
/// </summary>
public class BannedUser
{
    public int UserId { get; set; }

    public DateTime BannedAt { get; set; }

    public User User { get; set; } = null!;
}
