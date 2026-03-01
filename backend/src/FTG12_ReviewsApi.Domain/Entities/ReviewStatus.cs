namespace FTG12_ReviewsApi.Domain.Entities;

/// <summary>
/// Represents the moderation status of a review.
/// </summary>
public class ReviewStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Review> Reviews { get; set; } = [];
}
