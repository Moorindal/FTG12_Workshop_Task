using FluentAssertions;
using FTG12_ReviewsApi.Application.Auth.Queries;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Auth;

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
    public async Task Handle_WhenAuthenticated_ReturnsUserInfoWithBanStatus()
    {
        _currentUserService.UserId.Returns(1);
        var user = new User { Id = 1, Username = "Admin", IsAdministrator = true };
        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns((BannedUser?)null);

        var result = await _handler.Handle(new CurrentUserQuery(), CancellationToken.None);

        result.Id.Should().Be(1);
        result.Username.Should().Be("Admin");
        result.IsAdministrator.Should().BeTrue();
        result.IsBanned.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ThrowsUnauthorized()
    {
        _currentUserService.UserId.Returns((int?)null);

        Func<Task> act = () => _handler.Handle(new CurrentUserQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsUnauthorized()
    {
        _currentUserService.UserId.Returns(999);
        _userRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((User?)null);

        Func<Task> act = () => _handler.Handle(new CurrentUserQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WhenUserIsBanned_ReturnsBannedTrue()
    {
        _currentUserService.UserId.Returns(2);
        var user = new User { Id = 2, Username = "User1" };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new CurrentUserQuery(), CancellationToken.None);

        result.IsBanned.Should().BeTrue();
    }
}
