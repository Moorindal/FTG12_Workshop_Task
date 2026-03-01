import { useEffect, useState } from 'react';
import type { HealthStatus } from '../types/health';
import { fetchHealth } from '../services/apiClient';

/** The shape of the value returned by the {@link useHealthCheck} hook. */
interface UseHealthCheckResult {
  /** The health status data, or null if not yet loaded. */
  data: HealthStatus | null;

  /** Whether the health check request is currently in flight. */
  loading: boolean;

  /** An error message if the request failed, or null on success. */
  error: string | null;
}

/**
 * Custom hook that fetches the backend health status on mount.
 *
 * @returns An object containing `data`, `loading`, and `error` state.
 *
 * @example
 * ```tsx
 * const { data, loading, error } = useHealthCheck();
 * ```
 */
export function useHealthCheck(): UseHealthCheckResult {
  const [data, setData] = useState<HealthStatus | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();

    fetchHealth(controller.signal)
      .then((result) => {
        if (controller.signal.aborted) {
          return;
        }
        setData(result);
        setError(null);
      })
      .catch((err: unknown) => {
        if (controller.signal.aborted) {
          return;
        }
        const message = err instanceof Error ? err.message : 'Unknown error';
        setError(message);
      })
      .finally(() => {
        if (controller.signal.aborted) {
          return;
        }
        setLoading(false);
      });

    return () => {
      controller.abort();
    };
  }, []);

  return { data, loading, error };
}
