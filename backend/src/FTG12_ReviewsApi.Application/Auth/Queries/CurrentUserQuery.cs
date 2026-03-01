using FTG12_ReviewsApi.Application.Auth.DTOs;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Auth.Queries;

/// <summary>
/// Query to retrieve the currently authenticated user's information.
/// </summary>
public record CurrentUserQuery : IRequest<UserInfoDto>;

/// <summary>
/// Handles <see cref="CurrentUserQuery"/> by reading claims and checking ban status from DB.
/// </summary>
public class CurrentUserQueryHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IBannedUserRepository bannedUserRepository) : IRequestHandler<CurrentUserQuery, UserInfoDto>
{
    public async Task<UserInfoDto> Handle(CurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found.");

        var banned = await bannedUserRepository.GetByUserIdAsync(userId, cancellationToken);

        return new UserInfoDto(user.Id, user.Username, user.IsAdministrator, banned is not null);
    }
}
