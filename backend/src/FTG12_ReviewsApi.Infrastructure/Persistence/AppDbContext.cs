using FTG12_ReviewsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FTG12_ReviewsApi.Infrastructure.Persistence;

/// <summary>
/// Application database context for EF Core queries.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ReviewStatus> ReviewStatuses => Set<ReviewStatus>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<BannedUser> BannedUsers => Set<BannedUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
