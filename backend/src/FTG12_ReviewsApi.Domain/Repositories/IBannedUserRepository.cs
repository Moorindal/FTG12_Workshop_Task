using FTG12_ReviewsApi.Domain.Entities;

namespace FTG12_ReviewsApi.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="BannedUser"/> entity operations.
/// </summary>
public interface IBannedUserRepository
{
    Task<BannedUser?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<BannedUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BannedUser> AddAsync(BannedUser bannedUser, CancellationToken cancellationToken = default);
    Task RemoveAsync(int userId, CancellationToken cancellationToken = default);
}
