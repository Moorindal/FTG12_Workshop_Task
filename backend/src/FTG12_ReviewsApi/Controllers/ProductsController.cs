using FTG12_ReviewsApi.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTG12_ReviewsApi.Controllers;

/// <summary>
/// Endpoints for listing and retrieving products.
/// </summary>
[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns a paginated list of products ordered alphabetically.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetProductsQuery(page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns a single product by its ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }
}
