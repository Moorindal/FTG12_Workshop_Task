using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Users.DTOs;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Users.Commands;

/// <summary>
/// Command to unban a user by removing their BannedUser record.
/// </summary>
public record UnbanUserCommand(int UserId) : IRequest<UserDto>;

/// <summary>
/// Handles <see cref="UnbanUserCommand"/> by removing the ban record.
/// </summary>
public class UnbanUserCommandHandler(
    IUserRepository userRepository,
    IBannedUserRepository bannedUserRepository) : IRequestHandler<UnbanUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        var existing = await bannedUserRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existing is null)
        {
            throw new NotFoundException("BannedUser", request.UserId);
        }

        await bannedUserRepository.RemoveAsync(request.UserId, cancellationToken);

        return new UserDto(
            user.Id, user.Username, user.IsAdministrator,
            false, null, user.CreatedAt);
    }
}
