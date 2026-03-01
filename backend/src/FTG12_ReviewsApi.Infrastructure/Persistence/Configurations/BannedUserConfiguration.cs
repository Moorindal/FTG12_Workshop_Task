using FTG12_ReviewsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTG12_ReviewsApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for <see cref="BannedUser"/>.
/// </summary>
public class BannedUserConfiguration : IEntityTypeConfiguration<BannedUser>
{
    public void Configure(EntityTypeBuilder<BannedUser> builder)
    {
        builder.ToTable("BannedUsers");
        builder.HasKey(b => b.UserId);
        builder.Property(b => b.BannedAt).IsRequired();

        builder.HasOne(b => b.User)
            .WithOne()
            .HasForeignKey<BannedUser>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.UserId).IsUnique();
    }
}
