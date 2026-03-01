import { renderHook, waitFor, act } from '@testing-library/react';
import { useUsers } from './useUsers';
import { http, HttpResponse } from 'msw';
import { server } from '../test/server';
import { createUser } from '../test/factories';

describe('useUsers', () => {
  it('fetches users on mount', async () => {
    server.use(
      http.get('http://localhost:7100/api/admin/users', () => {
        return HttpResponse.json([
          createUser({ id: 1, username: 'alice' }),
          createUser({ id: 2, username: 'bob' }),
        ]);
      }),
    );
    const { result } = renderHook(() => useUsers());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.users).toHaveLength(2);
    expect(result.current.users[0].username).toBe('alice');
  });

  it('banUser updates user in list', async () => {
    server.use(
      http.get('http://localhost:7100/api/admin/users', () => {
        return HttpResponse.json([createUser({ id: 1, isBanned: false })]);
      }),
      http.post('http://localhost:7100/api/admin/users/1/ban', () => {
        return HttpResponse.json(createUser({ id: 1, isBanned: true, bannedAt: '2024-01-01T00:00:00Z' }));
      }),
    );
    const { result } = renderHook(() => useUsers());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.users[0].isBanned).toBe(false);
    await act(async () => {
      await result.current.banUser(1);
    });
    expect(result.current.users[0].isBanned).toBe(true);
  });

  it('unbanUser updates user in list', async () => {
    server.use(
      http.get('http://localhost:7100/api/admin/users', () => {
        return HttpResponse.json([createUser({ id: 1, isBanned: true, bannedAt: '2024-01-01T00:00:00Z' })]);
      }),
      http.post('http://localhost:7100/api/admin/users/1/unban', () => {
        return HttpResponse.json(createUser({ id: 1, isBanned: false, bannedAt: null }));
      }),
    );
    const { result } = renderHook(() => useUsers());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.users[0].isBanned).toBe(true);
    await act(async () => {
      await result.current.unbanUser(1);
    });
    expect(result.current.users[0].isBanned).toBe(false);
  });

  it('starts with loading true', () => {
    const { result } = renderHook(() => useUsers());
    expect(result.current.loading).toBe(true);
  });
});
