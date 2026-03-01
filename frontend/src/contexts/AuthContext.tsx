/* eslint-disable react-refresh/only-export-components */
import { createContext, useState, useEffect, useCallback, type ReactNode } from 'react';
import type { UserInfo } from '../types/auth';
import * as api from '../services/apiClient';

interface AuthContextType {
  user: UserInfo | null;
  token: string | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  loading: boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

function isTokenExpired(token: string): boolean {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const exp = payload.exp as number;
    return Date.now() >= exp * 1000;
  } catch {
    return true;
  }
}

function getInitialToken(): string | null {
  const stored = localStorage.getItem('token');
  if (stored && !isTokenExpired(stored)) return stored;
  if (stored) localStorage.removeItem('token');
  return null;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [token, setToken] = useState<string | null>(getInitialToken);
  const [loading, setLoading] = useState(() => getInitialToken() !== null);

  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    if (!storedToken || isTokenExpired(storedToken)) return;
    const controller = new AbortController();
    api.getCurrentUser(controller.signal)
      .then((u) => {
        setUser(u);
      })
      .catch((err: unknown) => {
        if (controller.signal.aborted) return;
        if (err instanceof DOMException && err.name === 'AbortError') return;
        localStorage.removeItem('token');
        setToken(null);
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });
    return () => controller.abort();
  }, []);

  const login = useCallback(async (username: string, password: string) => {
    const response = await api.login({ username, password });
    localStorage.setItem('token', response.token);
    setToken(response.token);
    setUser(response.user);
  }, []);

  const logout = useCallback(async () => {
    try {
      await api.logout();
    } catch {
      // ignore errors during logout
    }
    localStorage.removeItem('token');
    setToken(null);
    setUser(null);
  }, []);

  const isAuthenticated = token !== null && user !== null;
  const isAdmin = user?.isAdministrator ?? false;

  return (
    <AuthContext value={{ user, token, isAuthenticated, isAdmin, login, logout, loading }}>
      {children}
    </AuthContext>
  );
}
