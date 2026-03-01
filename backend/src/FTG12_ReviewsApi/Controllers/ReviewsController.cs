using FTG12_ReviewsApi.Application.Reviews.Commands;
using FTG12_ReviewsApi.Application.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTG12_ReviewsApi.Controllers;

/// <summary>
/// Endpoints for creating, updating, and listing reviews.
/// </summary>
[ApiController]
[Authorize]
public class ReviewsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new review for a product.
    /// </summary>
    [HttpPost("api/reviews")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateReviewAsync(
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReviewCommand(request.ProductId, request.Rating, request.Text);
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Updates an existing review owned by the current user.
    /// </summary>
    [HttpPut("api/reviews/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReviewAsync(
        int id,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateReviewCommand(id, request.Rating, request.Text);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns the current user's reviews across all statuses.
    /// </summary>
    [HttpGet("api/reviews/my")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyReviewsAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetMyReviewsQuery(page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns paginated approved reviews for a specific product.
    /// </summary>
    [HttpGet("api/products/{productId:int}/reviews")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReviewsByProductAsync(
        int productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetReviewsByProductQuery(productId, page, pageSize), cancellationToken);
        return Ok(result);
    }
}

/// <summary>
/// Request body for creating a review.
/// </summary>
public record CreateReviewRequest(int ProductId, int Rating, string Text);

/// <summary>
/// Request body for updating a review.
/// </summary>
public record UpdateReviewRequest(int Rating, string Text);
