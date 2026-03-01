using FTG12_ReviewsApi.Application.Reviews.Commands;
using FTG12_ReviewsApi.Application.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTG12_ReviewsApi.Controllers;

/// <summary>
/// Admin-only endpoints for managing reviews.
/// </summary>
[ApiController]
[Route("api/admin/reviews")]
[Authorize(Roles = "Admin")]
public class AdminReviewsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns all reviews with optional filtering by status and date range.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllReviewsAsync(
        [FromQuery] int? statusId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetAllReviewsQuery(statusId, dateFrom, dateTo, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Changes a review's moderation status.
    /// </summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeReviewStatusAsync(
        int id,
        [FromBody] ChangeStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ChangeReviewStatusCommand(id, request.StatusId),
            cancellationToken);
        return Ok(result);
    }
}

/// <summary>
/// Request body for changing review status.
/// </summary>
public record ChangeStatusRequest(int StatusId);
