using FTG12_ReviewsApi.Application.Auth.Commands;
using FTG12_ReviewsApi.Application.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTG12_ReviewsApi.Controllers;

/// <summary>
/// Authentication endpoints: login, logout, and current user.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Logs out the current user (stateless — frontend discards token).
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully." });
    }

    /// <summary>
    /// Returns the current authenticated user's information including ban status.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CurrentUserQuery(), cancellationToken);
        return Ok(result);
    }
}
