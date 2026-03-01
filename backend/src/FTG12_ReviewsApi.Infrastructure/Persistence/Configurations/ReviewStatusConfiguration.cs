using FTG12_ReviewsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTG12_ReviewsApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for <see cref="ReviewStatus"/>.
/// </summary>
public class ReviewStatusConfiguration : IEntityTypeConfiguration<ReviewStatus>
{
    public void Configure(EntityTypeBuilder<ReviewStatus> builder)
    {
        builder.ToTable("ReviewStatuses");
        builder.HasKey(rs => rs.Id);
        builder.Property(rs => rs.Name).IsRequired();
    }
}
