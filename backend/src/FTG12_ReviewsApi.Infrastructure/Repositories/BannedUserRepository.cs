using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using FTG12_ReviewsApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FTG12_ReviewsApi.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IBannedUserRepository"/>.
/// </summary>
public class BannedUserRepository(AppDbContext context) : IBannedUserRepository
{
    public async Task<BannedUser?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        await context.BannedUsers.FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);

    public async Task<List<BannedUser>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.BannedUsers
            .Include(b => b.User)
            .ToListAsync(cancellationToken);

    public async Task<BannedUser> AddAsync(BannedUser bannedUser, CancellationToken cancellationToken = default)
    {
        context.BannedUsers.Add(bannedUser);
        await context.SaveChangesAsync(cancellationToken);
        return bannedUser;
    }

    public async Task RemoveAsync(int userId, CancellationToken cancellationToken = default)
    {
        var banned = await context.BannedUsers.FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);
        if (banned is not null)
        {
            context.BannedUsers.Remove(banned);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
