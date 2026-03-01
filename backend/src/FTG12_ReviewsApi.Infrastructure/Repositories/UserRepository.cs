using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using FTG12_ReviewsApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FTG12_ReviewsApi.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserRepository"/>.
/// </summary>
public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await context.Users.FindAsync([id], cancellationToken);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Users.ToListAsync(cancellationToken);

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
    }
}
