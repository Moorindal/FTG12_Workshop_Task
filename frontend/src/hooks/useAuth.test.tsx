import { renderHook } from '@testing-library/react';
import { useAuth } from './useAuth';
import { AuthContext } from '../contexts/AuthContext';
import type { ReactNode } from 'react';

describe('useAuth', () => {
  it('throws when used outside AuthProvider', () => {
    // Suppress console.error for expected error
    const spy = vi.spyOn(console, 'error').mockImplementation(() => {});
    expect(() => {
      renderHook(() => useAuth());
    }).toThrow('useAuth must be used within an AuthProvider');
    spy.mockRestore();
  });

  it('returns context value when inside provider', () => {
    const value = {
      user: null,
      token: null,
      isAuthenticated: false,
      isAdmin: false,
      login: vi.fn(),
      logout: vi.fn(),
      loading: false,
    };
    const wrapper = ({ children }: { children: ReactNode }) => (
      <AuthContext value={value}>{children}</AuthContext>
    );
    const { result } = renderHook(() => useAuth(), { wrapper });
    expect(result.current.isAuthenticated).toBe(false);
    expect(result.current.loading).toBe(false);
  });
});
