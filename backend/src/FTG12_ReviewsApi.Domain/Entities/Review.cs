namespace FTG12_ReviewsApi.Domain.Entities;

/// <summary>
/// Represents a user's review for a product.
/// </summary>
public class Review
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int StatusId { get; set; }

    public int Rating { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Product Product { get; set; } = null!;

    public User User { get; set; } = null!;

    public ReviewStatus Status { get; set; } = null!;
}
