using FTG12_ReviewsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTG12_ReviewsApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for <see cref="User"/>.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.IsAdministrator).HasDefaultValue(false);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.HasIndex(u => u.Username).IsUnique();
    }
}
