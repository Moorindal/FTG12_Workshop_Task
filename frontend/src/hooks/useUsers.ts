import { useState, useEffect, useCallback } from 'react';
import type { User } from '../types/user';
import * as api from '../services/apiClient';

interface UseUsersResult {
  users: User[];
  loading: boolean;
  error: string | null;
  banUser: (userId: number) => Promise<void>;
  unbanUser: (userId: number) => Promise<void>;
  refresh: () => void;
}

export function useUsers(): UseUsersResult {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [refreshCounter, setRefreshCounter] = useState(0);

  useEffect(() => {
    const controller = new AbortController();
    api.getUsers(controller.signal)
      .then((result) => {
        if (!controller.signal.aborted) setUsers(result);
      })
      .catch((err: unknown) => {
        if (!controller.signal.aborted) setError(err instanceof Error ? err.message : 'Failed to load users');
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });
    return () => controller.abort();
  }, [refreshCounter]);

  const ban = useCallback(async (userId: number) => {
    const updated = await api.banUser(userId);
    setUsers((prev) => prev.map((u) => (u.id === userId ? updated : u)));
  }, []);

  const unban = useCallback(async (userId: number) => {
    const updated = await api.unbanUser(userId);
    setUsers((prev) => prev.map((u) => (u.id === userId ? updated : u)));
  }, []);

  const refresh = useCallback(() => {
    setLoading(true);
    setError(null);
    setRefreshCounter((c) => c + 1);
  }, []);

  return { users, loading, error, banUser: ban, unbanUser: unban, refresh };
}
