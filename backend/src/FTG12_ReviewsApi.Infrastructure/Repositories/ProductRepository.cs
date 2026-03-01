using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using FTG12_ReviewsApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FTG12_ReviewsApi.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IProductRepository"/>.
/// </summary>
public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await context.Products.FindAsync([id], cancellationToken);

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Products.ToListAsync(cancellationToken);
}
