using FTG12_ReviewsApi.Domain.Entities;

namespace FTG12_ReviewsApi.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="Product"/> entity operations.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
}
