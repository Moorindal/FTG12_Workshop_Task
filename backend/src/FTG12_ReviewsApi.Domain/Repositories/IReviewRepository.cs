using FTG12_ReviewsApi.Domain.Entities;

namespace FTG12_ReviewsApi.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="Review"/> entity operations.
/// </summary>
public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Review?> GetByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken = default);
    Task<List<Review>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<List<Review>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<Review>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
}
