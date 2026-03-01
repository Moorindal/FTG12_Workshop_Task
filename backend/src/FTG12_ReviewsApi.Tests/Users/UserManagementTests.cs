using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Users.Commands;
using FTG12_ReviewsApi.Application.Users.Queries;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Tests.Users;

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
    public async Task WhenUsersExistThenReturnsWithBanStatus()
    {
        var users = new List<User>
        {
            new() { Id = 1, Username = "Admin", IsAdministrator = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Username = "User1", IsAdministrator = false, CreatedAt = DateTime.UtcNow }
        };
        _userRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(users);

        var bannedAt = DateTime.UtcNow;
        var bannedUsers = new List<BannedUser>
        {
            new() { UserId = 2, BannedAt = bannedAt, User = users[1] }
        };
        _bannedUserRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(bannedUsers);

        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.False(result[0].IsBanned);
        Assert.True(result[1].IsBanned);
        Assert.Equal(bannedAt, result[1].BannedAt);
    }
}

public class BanUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IBannedUserRepository _bannedUserRepository = Substitute.For<IBannedUserRepository>();
    private readonly BanUserCommandHandler _handler;

    public BanUserCommandHandlerTests()
    {
        _handler = new BanUserCommandHandler(_userRepository, _bannedUserRepository);
    }

    [Fact]
    public async Task WhenUserExistsAndNotBannedThenBansUser()
    {
        var user = new User { Id = 2, Username = "User1", CreatedAt = DateTime.UtcNow };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>()).Returns((BannedUser?)null);
        _bannedUserRepository.AddAsync(Arg.Any<BannedUser>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<BannedUser>());

        var result = await _handler.Handle(new BanUserCommand(2), CancellationToken.None);

        Assert.True(result.IsBanned);
        Assert.NotNull(result.BannedAt);
    }

    [Fact]
    public async Task WhenUserNotFoundThenThrowsNotFound()
    {
        _userRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new BanUserCommand(999), CancellationToken.None));
    }

    [Fact]
    public async Task WhenUserAlreadyBannedThenThrowsConflict()
    {
        var user = new User { Id = 2, Username = "User1" };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        await Assert.ThrowsAsync<ConflictException>(
            () => _handler.Handle(new BanUserCommand(2), CancellationToken.None));
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
    public async Task WhenUserIsBannedThenUnbansUser()
    {
        var user = new User { Id = 2, Username = "User1", CreatedAt = DateTime.UtcNow };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(new BannedUser { UserId = 2, BannedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new UnbanUserCommand(2), CancellationToken.None);

        Assert.False(result.IsBanned);
        Assert.Null(result.BannedAt);
    }

    [Fact]
    public async Task WhenUserNotFoundThenThrowsNotFound()
    {
        _userRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new UnbanUserCommand(999), CancellationToken.None));
    }

    [Fact]
    public async Task WhenUserNotBannedThenThrowsNotFound()
    {
        var user = new User { Id = 2, Username = "User1" };
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _bannedUserRepository.GetByUserIdAsync(2, Arg.Any<CancellationToken>()).Returns((BannedUser?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new UnbanUserCommand(2), CancellationToken.None));
    }
}
