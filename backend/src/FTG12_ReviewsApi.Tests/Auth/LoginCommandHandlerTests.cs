using FTG12_ReviewsApi.Application.Auth.Commands;
using FTG12_ReviewsApi.Application.Auth.DTOs;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IBannedUserRepository _bannedUserRepository = Substitute.For<IBannedUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_userRepository, _bannedUserRepository, _passwordHasher, _jwtTokenService);
    }

    [Fact]
    public async Task WhenValidCredentialsThenReturnsTokenAndUserInfo()
    {
        var user = new User { Id = 1, Username = "Admin", PasswordHash = "hash", IsAdministrator = true };
        _userRepository.GetByUsernameAsync("Admin", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("Admin", "hash").Returns(true);
        _jwtTokenService.GenerateToken(user).Returns("jwt-token");
        _bannedUserRepository.GetByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns((BannedUser?)null);

        var result = await _handler.Handle(new LoginCommand("Admin", "Admin"), CancellationToken.None);

        Assert.Equal("jwt-token", result.Token);
        Assert.Equal(1, result.User.Id);
        Assert.Equal("Admin", result.User.Username);
        Assert.True(result.User.IsAdministrator);
        Assert.False(result.User.IsBanned);
    }

    [Fact]
    public async Task WhenInvalidUsernameThenThrowsUnauthorized()
    {
        _userRepository.GetByUsernameAsync("unknown", Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(new LoginCommand("unknown", "pass"), CancellationToken.None));
    }

    [Fact]
    public async Task WhenInvalidPasswordThenThrowsUnauthorized()
    {
        var user = new User { Id = 1, Username = "Admin", PasswordHash = "hash" };
        _userRepository.GetByUsernameAsync("Admin", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("wrong", "hash").Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(new LoginCommand("Admin", "wrong"), CancellationToken.None));
    }

    [Fact]
    public async Task WhenBannedUserLoginsThenIsBannedIsTrue()
    {
        var user = new User { Id = 2, Username = "User1", PasswordHash = "hash", IsAdministrator = false };
        _userRepository.GetByUsernameAsync("User1", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("User1", "hash").Returns(true);
        _jwtTokenService.GenerateToken(user).Returns("jwt-token");
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new LoginCommand("User1", "User1"), CancellationToken.None);

        Assert.True(result.User.IsBanned);
    }
}
