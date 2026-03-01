/**
 * TypeScript interface representing the health status response from the backend API.
 */
export interface HealthStatus {
  /** A human-readable health status string (e.g., "Healthy"). */
  status: string;

  /** The UTC timestamp when the health check was performed. */
  timestamp: string;
}
