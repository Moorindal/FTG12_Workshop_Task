namespace FTG12_ReviewsApi.Models;

/// <summary>
/// Represents the health status response returned by the health check endpoint.
/// </summary>
/// <param name="Status">A human-readable health status string (e.g., "Healthy").</param>
/// <param name="Timestamp">The UTC date and time when the health check was performed.</param>
public record HealthStatus(string Status, DateTime Timestamp);
