using FTG12_ReviewsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FTG12_ReviewsApi.Controllers;

/// <summary>
/// Provides a simple health check endpoint for monitoring the API status.
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Returns the current health status of the API.
    /// </summary>
    /// <returns>
    /// An <see cref="HealthStatus"/> object containing the status string and a UTC timestamp.
    /// </returns>
    /// <response code="200">The API is healthy and operational.</response>
    /// <example>
    /// <code>
    /// GET /health
    /// Response: { "status": "Healthy", "timestamp": "2026-03-01T12:00:00Z" }
    /// </code>
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(HealthStatus), StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        var status = new HealthStatus("Healthy", DateTime.UtcNow);
        return Ok(status);
    }
}
