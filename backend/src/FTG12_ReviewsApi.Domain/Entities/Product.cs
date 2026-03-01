namespace FTG12_ReviewsApi.Domain.Entities;

/// <summary>
/// Represents a product that can be reviewed.
/// </summary>
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Review> Reviews { get; set; } = [];
}
