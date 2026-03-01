using FluentAssertions;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Application.Users.Commands;
using FTG12_ReviewsApi.Application.Users.Queries;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Users;

public class GetUsersQueryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IBannedUserRepository _bannedUserRepository = Substitute.For<IBannedUserRepository>();
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _handler = new GetUsersQueryHandler(_userRepository, _bannedUserRepository);
    }

    [Fact]
    public async Task Handle_ReturnsUsersWithBanStatus()
    {
        var users = new List<User>
        {
            new() { Id = 1, Username = "Admin", IsAdministrator = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Username = "User1", IsAdministrator = false, CreatedAt = DateTime.UtcNow }
        };
        _userRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(users);

        var bannedAt = DateTime.UtcNow;
        _bannedUserRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(
            new List<BannedUser> { new() { UserId = 2, BannedAt = bannedAt, User = users[1] } });

        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].IsBanned.Should().BeFalse();
        result[1].IsBanned.Should().BeTrue();
        result[1].BannedAt.Should().Be(bannedAt);
    }
}

public class BanUserCommandHandlerTests
{
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IBannedUserRepository _bannedUserRepository = Substitute.For<IBannedUserRepository>();
    private readonly BanUserCommandHandler _handler;

    public BanUserCommandHandlerTests()
    {
        _currentUserService.UserId.Returns(1);
        _handler = new BanUserCommandHandler(_currentUserService, _userRepository, _bannedUserRepository);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndNotBanned_BansUser()
    {
        var user = new User { Id = 2, Username = "User1", CreatedAt = DateTime.UtcNow };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>()).Returns((BannedUser?)null);
        _bannedUserRepository.AddAsync(Arg.Any<BannedUser>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<BannedUser>());

        var result = await _handler.Handle(new BanUserCommand(2), CancellationToken.None);

        result.IsBanned.Should().BeTrue();
        result.BannedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsNotFound()
    {
        _userRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((User?)null);

        Func<Task> act = () => _handler.Handle(new BanUserCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyBanned_ThrowsConflict()
    {
        var user = new User { Id = 2, Username = "User1" };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        Func<Task> act = () => _handler.Handle(new BanUserCommand(2), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_WhenBanningSelf_ThrowsForbidden()
    {
        _currentUserService.UserId.Returns(1);
        var user = new User { Id = 1, Username = "Admin", IsAdministrator = true };
        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        Func<Task> act = () => _handler.Handle(new BanUserCommand(1), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("*cannot ban themselves*");
    }

    [Fact]
    public async Task Handle_WhenBanningAdmin_ThrowsForbidden()
    {
        _currentUserService.UserId.Returns(1);
        var user = new User { Id = 5, Username = "Admin2", IsAdministrator = true };
        _userRepository.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(user);

        Func<Task> act = () => _handler.Handle(new BanUserCommand(5), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("*cannot ban other administrators*");
    }
}

public class UnbanUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IBannedUserRepository _bannedUserRepository = Substitute.For<IBannedUserRepository>();
    private readonly UnbanUserCommandHandler _handler;

    public UnbanUserCommandHandlerTests()
    {
        _handler = new UnbanUserCommandHandler(_userRepository, _bannedUserRepository);
    }

    [Fact]
    public async Task Handle_WhenUserIsBanned_UnbansUser()
    {
        var user = new User { Id = 2, Username = "User1", CreatedAt = DateTime.UtcNow };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new UnbanUserCommand(2), CancellationToken.None);

        result.IsBanned.Should().BeFalse();
        result.BannedAt.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsNotFound()
    {
        _userRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((User?)null);

        Func<Task> act = () => _handler.Handle(new UnbanUserCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenUserNotBanned_ThrowsNotFound()
    {
        var user = new User { Id = 2, Username = "User1" };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>()).Returns((BannedUser?)null);

        Func<Task> act = () => _handler.Handle(new UnbanUserCommand(2), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
