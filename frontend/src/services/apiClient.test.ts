import { http, HttpResponse } from 'msw';
import { server } from '../test/server';
import { createJwt } from '../test/factories';
import * as api from './apiClient';

describe('apiClient', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('attaches auth token to requests', async () => {
    const token = createJwt();
    localStorage.setItem('token', token);
    let capturedAuth = '';
    server.use(
      http.get('http://localhost:7100/api/products', ({ request }) => {
        capturedAuth = request.headers.get('Authorization') ?? '';
        return HttpResponse.json({ items: [], page: 1, pageSize: 10, totalCount: 0, totalPages: 0 });
      }),
    );
    await api.getProducts(1, 10);
    expect(capturedAuth).toBe(`Bearer ${token}`);
  });

  it('does not attach auth header when no token', async () => {
    let capturedAuth: string | null = 'initial';
    server.use(
      http.get('http://localhost:7100/api/products', ({ request }) => {
        capturedAuth = request.headers.get('Authorization');
        return HttpResponse.json({ items: [], page: 1, pageSize: 10, totalCount: 0, totalPages: 0 });
      }),
    );
    await api.getProducts(1, 10);
    expect(capturedAuth).toBeNull();
  });

  it('handles 401 by clearing token', async () => {
    localStorage.setItem('token', createJwt());
    // Prevent actual navigation
    const origHref = window.location.href;
    Object.defineProperty(window, 'location', { value: { href: origHref }, writable: true, configurable: true });
    server.use(
      http.get('http://localhost:7100/api/products', () => {
        return new HttpResponse(null, { status: 401 });
      }),
    );
    await expect(api.getProducts(1, 10)).rejects.toThrow('Unauthorized');
    expect(localStorage.getItem('token')).toBeNull();
  });

  it('parses ProblemDetails error response', async () => {
    server.use(
      http.post('http://localhost:7100/api/reviews', () => {
        return HttpResponse.json(
          { title: 'Validation Error', detail: 'Rating is required', status: 400 },
          { status: 400 },
        );
      }),
    );
    await expect(api.createReview(1, 0, '')).rejects.toThrow('Rating is required');
  });

  it('formats POST request body correctly', async () => {
    let capturedBody: Record<string, unknown> | null = null;
    server.use(
      http.post('http://localhost:7100/api/auth/login', async ({ request }) => {
        capturedBody = await request.json() as Record<string, unknown>;
        return HttpResponse.json({ token: createJwt(), user: { id: 1, username: 'u', isAdministrator: false, isBanned: false } });
      }),
    );
    await api.login({ username: 'Admin', password: 'Admin' });
    expect(capturedBody).toEqual({ username: 'Admin', password: 'Admin' });
  });

  it('correctly parses JSON response', async () => {
    const result = await api.getProducts(1, 10);
    expect(result).toHaveProperty('items');
    expect(result).toHaveProperty('totalPages');
    expect(Array.isArray(result.items)).toBe(true);
  });

  it('handles 204 No Content response', async () => {
    localStorage.setItem('token', createJwt());
    const result = await api.logout();
    expect(result).toBeUndefined();
  });
});
