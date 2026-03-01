using FTG12_ReviewsApi.Application.Users.DTOs;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Users.Queries;

/// <summary>
/// Query to retrieve all users with their ban status.
/// </summary>
public record GetUsersQuery : IRequest<List<UserDto>>;

/// <summary>
/// Handles <see cref="GetUsersQuery"/> by joining users with banned user records.
/// </summary>
public class GetUsersQueryHandler(
    IUserRepository userRepository,
    IBannedUserRepository bannedUserRepository) : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        var bannedUsers = await bannedUserRepository.GetAllAsync(cancellationToken);
        var bannedLookup = bannedUsers.ToDictionary(b => b.UserId, b => b.BannedAt);

        return users.Select(u => new UserDto(
            u.Id,
            u.Username,
            u.IsAdministrator,
            bannedLookup.ContainsKey(u.Id),
            bannedLookup.TryGetValue(u.Id, out var bannedAt) ? bannedAt : null,
            u.CreatedAt))
            .ToList();
    }
}
