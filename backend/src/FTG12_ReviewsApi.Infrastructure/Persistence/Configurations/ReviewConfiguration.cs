using FTG12_ReviewsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTG12_ReviewsApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for <see cref="Review"/>.
/// </summary>
public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Rating).IsRequired();
        builder.Property(r => r.Text).IsRequired().HasMaxLength(8000);
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Status)
            .WithMany(s => s.Reviews)
            .HasForeignKey(r => r.StatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.StatusId);
        builder.HasIndex(r => r.CreatedAt);
        builder.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
    }
}
