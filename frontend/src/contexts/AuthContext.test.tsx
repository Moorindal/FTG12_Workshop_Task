import { render, screen, act, waitFor } from '@testing-library/react';
import { AuthProvider, AuthContext } from './AuthContext';
import { useContext } from 'react';
import { createJwt, createUserInfo } from '../test/factories';
import { http, HttpResponse } from 'msw';
import { server } from '../test/server';

function TestConsumer() {
  const ctx = useContext(AuthContext);
  if (!ctx) return <div>no context</div>;
  return (
    <div>
      <span data-testid="authenticated">{String(ctx.isAuthenticated)}</span>
      <span data-testid="admin">{String(ctx.isAdmin)}</span>
      <span data-testid="loading">{String(ctx.loading)}</span>
      <span data-testid="username">{ctx.user?.username ?? 'none'}</span>
      <button onClick={() => ctx.login('Admin', 'Admin')}>login</button>
      <button onClick={() => ctx.logout()}>logout</button>
    </div>
  );
}

describe('AuthContext', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('provides default unauthenticated state', () => {
    render(<AuthProvider><TestConsumer /></AuthProvider>);
    expect(screen.getByTestId('authenticated')).toHaveTextContent('false');
    expect(screen.getByTestId('username')).toHaveTextContent('none');
  });

  it('login() sets user and token', async () => {
    const jwt = createJwt();
    const user = createUserInfo({ username: 'Admin' });
    server.use(
      http.post('http://localhost:7100/api/auth/login', () => {
        return HttpResponse.json({ token: jwt, user });
      }),
    );
    render(<AuthProvider><TestConsumer /></AuthProvider>);
    await act(async () => { screen.getByText('login').click(); });
    await waitFor(() => {
      expect(screen.getByTestId('authenticated')).toHaveTextContent('true');
    });
    expect(screen.getByTestId('username')).toHaveTextContent('Admin');
    expect(localStorage.getItem('token')).toBe(jwt);
  });

  it('logout() clears user and token', async () => {
    const jwt = createJwt();
    const user = createUserInfo();
    server.use(
      http.post('http://localhost:7100/api/auth/login', () => {
        return HttpResponse.json({ token: jwt, user });
      }),
    );
    render(<AuthProvider><TestConsumer /></AuthProvider>);
    await act(async () => { screen.getByText('login').click(); });
    await waitFor(() => {
      expect(screen.getByTestId('authenticated')).toHaveTextContent('true');
    });
    await act(async () => { screen.getByText('logout').click(); });
    await waitFor(() => {
      expect(screen.getByTestId('authenticated')).toHaveTextContent('false');
    });
    expect(localStorage.getItem('token')).toBeNull();
  });

  it('restores session from localStorage on mount', async () => {
    const jwt = createJwt();
    localStorage.setItem('token', jwt);
    server.use(
      http.get('http://localhost:7100/api/auth/me', () => {
        return HttpResponse.json(createUserInfo({ username: 'Restored' }));
      }),
    );
    render(<AuthProvider><TestConsumer /></AuthProvider>);
    await waitFor(() => {
      expect(screen.getByTestId('loading')).toHaveTextContent('false');
    });
    expect(screen.getByTestId('authenticated')).toHaveTextContent('true');
    expect(screen.getByTestId('username')).toHaveTextContent('Restored');
  });
});
