using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Users.DTOs;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Users.Commands;

/// <summary>
/// Command to ban a user by creating a BannedUser record.
/// </summary>
public record BanUserCommand(int UserId) : IRequest<UserDto>;

/// <summary>
/// Handles <see cref="BanUserCommand"/> by adding a ban record.
/// </summary>
public class BanUserCommandHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IBannedUserRepository bannedUserRepository) : IRequestHandler<BanUserCommand, UserDto>
{
    public async Task<UserDto> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        if (request.UserId == currentUserService.UserId)
        {
            throw new ForbiddenException("Administrators cannot ban themselves.");
        }

        if (user.IsAdministrator)
        {
            throw new ForbiddenException("Administrators cannot ban other administrators.");
        }

        var existing = await bannedUserRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException("User is already banned.");
        }

        var bannedUser = new BannedUser
        {
            UserId = request.UserId,
            BannedAt = DateTime.UtcNow
        };

        await bannedUserRepository.AddAsync(bannedUser, cancellationToken);

        return new UserDto(
            user.Id, user.Username, user.IsAdministrator,
            true, bannedUser.BannedAt, user.CreatedAt);
    }
}
