import type { HealthStatus } from '../types/health';

/** Base URL for the backend API. */
const API_BASE_URL = 'http://localhost:7100';

/**
 * Fetches the health status from the backend API.
 *
 * @returns A promise that resolves to the {@link HealthStatus} response.
 * @throws An error if the network request fails or the response is not OK.
 */
export async function fetchHealth(): Promise<HealthStatus> {
  const response = await fetch(`${API_BASE_URL}/health`);

  if (!response.ok)
  {
    throw new Error(`Health check failed with status ${response.status}`);
  }

  return response.json() as Promise<HealthStatus>;
}
