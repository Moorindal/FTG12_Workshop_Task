using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using FTG12_ReviewsApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FTG12_ReviewsApi.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IReviewRepository"/>.
/// </summary>
public class ReviewRepository(AppDbContext context) : IReviewRepository
{
    public async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await context.Reviews
            .Include(r => r.Product)
            .Include(r => r.User)
            .Include(r => r.Status)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Review?> GetByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken = default) =>
        await context.Reviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId, cancellationToken);

    public async Task<List<Review>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default) =>
        await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Status)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<List<Review>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        await context.Reviews
            .Include(r => r.Product)
            .Include(r => r.Status)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<List<Review>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Reviews
            .Include(r => r.Product)
            .Include(r => r.User)
            .Include(r => r.Status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        context.Reviews.Add(review);
        await context.SaveChangesAsync(cancellationToken);
        return review;
    }

    public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        context.Reviews.Update(review);
        await context.SaveChangesAsync(cancellationToken);
    }
}
