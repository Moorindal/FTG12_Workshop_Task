using FluentAssertions;
using FTG12_ReviewsApi.Application.Auth.Commands;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IBannedUserRepository _bannedUserRepository = Substitute.For<IBannedUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(
            _userRepository, _bannedUserRepository, _passwordHasher, _jwtTokenService);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsTokenAndUserInfo()
    {
        var user = new User { Id = 1, Username = "Admin", PasswordHash = "hash", IsAdministrator = true };
        _userRepository.GetByUsernameAsync("Admin", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("Admin", "hash").Returns(true);
        _jwtTokenService.GenerateToken(user).Returns("jwt-token");
        _bannedUserRepository.GetByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns((BannedUser?)null);

        var result = await _handler.Handle(new LoginCommand("Admin", "Admin"), CancellationToken.None);

        result.Token.Should().Be("jwt-token");
        result.User.Id.Should().Be(1);
        result.User.Username.Should().Be("Admin");
        result.User.IsAdministrator.Should().BeTrue();
        result.User.IsBanned.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithInvalidUsername_ThrowsUnauthorized()
    {
        _userRepository.GetByUsernameAsync("unknown", Arg.Any<CancellationToken>()).Returns((User?)null);

        Func<Task> act = () => _handler.Handle(new LoginCommand("unknown", "pass"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ThrowsUnauthorized()
    {
        var user = new User { Id = 1, Username = "Admin", PasswordHash = "hash" };
        _userRepository.GetByUsernameAsync("Admin", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("wrong", "hash").Returns(false);

        Func<Task> act = () => _handler.Handle(new LoginCommand("Admin", "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithCaseInsensitiveUsername_DelegatesToRepository()
    {
        var user = new User { Id = 1, Username = "Admin", PasswordHash = "hash", IsAdministrator = true };
        _userRepository.GetByUsernameAsync("admin", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("Admin", "hash").Returns(true);
        _jwtTokenService.GenerateToken(user).Returns("jwt-token");
        _bannedUserRepository.GetByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns((BannedUser?)null);

        var result = await _handler.Handle(new LoginCommand("admin", "Admin"), CancellationToken.None);

        result.Token.Should().Be("jwt-token");
        await _userRepository.Received(1).GetByUsernameAsync("admin", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserIsBanned_ReturnsBannedStatus()
    {
        var user = new User { Id = 2, Username = "User1", PasswordHash = "hash", IsAdministrator = false };
        _userRepository.GetByUsernameAsync("User1", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("User1", "hash").Returns(true);
        _jwtTokenService.GenerateToken(user).Returns("jwt-token");
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new LoginCommand("User1", "User1"), CancellationToken.None);

        result.User.IsBanned.Should().BeTrue();
        result.Token.Should().NotBeEmpty();
    }
}
