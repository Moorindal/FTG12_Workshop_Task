using FluentValidation;
using FTG12_ReviewsApi.Application.Auth.DTOs;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Auth.Commands;

/// <summary>
/// Command to authenticate a user with username and password.
/// </summary>
public record LoginCommand(string Username, string Password) : IRequest<LoginResponse>;

/// <summary>
/// Validates <see cref="LoginCommand"/> inputs.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

/// <summary>
/// Handles user login by verifying credentials and generating a JWT token.
/// </summary>
public class LoginCommandHandler(
    IUserRepository userRepository,
    IBannedUserRepository bannedUserRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var banned = await bannedUserRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var token = jwtTokenService.GenerateToken(user);

        var userInfo = new UserInfoDto(user.Id, user.Username, user.IsAdministrator, banned is not null);
        return new LoginResponse(token, userInfo);
    }
}
