using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that prevents banned users from executing commands
/// marked with <see cref="IBannedUserCheck"/>.
/// </summary>
public class BannedUserBehavior<TRequest, TResponse>(
    ICurrentUserService currentUserService,
    IBannedUserRepository bannedUserRepository) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBannedUserCheck
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        if (userId.HasValue)
        {
            var banned = await bannedUserRepository.GetByUserIdAsync(userId.Value, cancellationToken);
            if (banned is not null)
            {
                throw new ForbiddenException("Your account has been banned. You cannot create or update reviews.");
            }
        }

        return await next(cancellationToken);
    }
}
