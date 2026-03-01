using FTG12_ReviewsApi.Application.Auth.Queries;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Tests.Auth;

public class CurrentUserQueryHandlerTests
{
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IBannedUserRepository _bannedUserRepository = Substitute.For<IBannedUserRepository>();
    private readonly CurrentUserQueryHandler _handler;

    public CurrentUserQueryHandlerTests()
    {
        _handler = new CurrentUserQueryHandler(_currentUserService, _userRepository, _bannedUserRepository);
    }

    [Fact]
    public async Task WhenAuthenticatedThenReturnsUserInfo()
    {
        _currentUserService.UserId.Returns(1);
        var user = new User { Id = 1, Username = "Admin", IsAdministrator = true };
        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns((BannedUser?)null);

        var result = await _handler.Handle(new CurrentUserQuery(), CancellationToken.None);

        Assert.Equal(1, result.Id);
        Assert.Equal("Admin", result.Username);
        Assert.True(result.IsAdministrator);
        Assert.False(result.IsBanned);
    }

    [Fact]
    public async Task WhenNotAuthenticatedThenThrowsUnauthorized()
    {
        _currentUserService.UserId.Returns((int?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(new CurrentUserQuery(), CancellationToken.None));
    }

    [Fact]
    public async Task WhenUserBannedThenIsBannedIsTrue()
    {
        _currentUserService.UserId.Returns(2);
        var user = new User { Id = 2, Username = "User1" };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new CurrentUserQuery(), CancellationToken.None);

        Assert.True(result.IsBanned);
    }
}
