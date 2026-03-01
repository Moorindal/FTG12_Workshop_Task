using FTG12_ReviewsApi.Application.Users.Commands;
using FTG12_ReviewsApi.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTG12_ReviewsApi.Controllers;

/// <summary>
/// Admin-only endpoints for user management (list, ban, unban).
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns all users with their ban status.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUsersQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Bans a user by creating a BannedUser record.
    /// </summary>
    [HttpPost("{id:int}/ban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BanUserAsync(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new BanUserCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Unbans a user by removing their BannedUser record.
    /// </summary>
    [HttpPost("{id:int}/unban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnbanUserAsync(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UnbanUserCommand(id), cancellationToken);
        return Ok(result);
    }
}
