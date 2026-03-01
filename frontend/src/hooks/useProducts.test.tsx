import { renderHook, waitFor } from '@testing-library/react';
import { useProducts } from './useProducts';
import { http, HttpResponse } from 'msw';
import { server } from '../test/server';
import { createProduct, createPaginatedResponse } from '../test/factories';

describe('useProducts', () => {
  it('starts with loading true', () => {
    const { result } = renderHook(() => useProducts());
    expect(result.current.loading).toBe(true);
    expect(result.current.products).toEqual([]);
  });

  it('fetches products successfully', async () => {
    server.use(
      http.get('http://localhost:7100/api/products', () => {
        return HttpResponse.json(
          createPaginatedResponse([
            createProduct({ id: 1, name: 'A' }),
            createProduct({ id: 2, name: 'B' }),
          ], { totalPages: 1 }),
        );
      }),
    );
    const { result } = renderHook(() => useProducts());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.products).toHaveLength(2);
    expect(result.current.error).toBeNull();
  });

  it('sets error on failure', async () => {
    server.use(
      http.get('http://localhost:7100/api/products', () => {
        return HttpResponse.json({ title: 'Server Error', status: 500 }, { status: 500 });
      }),
    );
    const { result } = renderHook(() => useProducts());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.error).toBeTruthy();
    expect(result.current.products).toEqual([]);
  });

  it('returns page and totalPages', async () => {
    server.use(
      http.get('http://localhost:7100/api/products', () => {
        return HttpResponse.json(
          createPaginatedResponse([createProduct()], { page: 1, totalPages: 3 }),
        );
      }),
    );
    const { result } = renderHook(() => useProducts());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.page).toBe(1);
    expect(result.current.totalPages).toBe(3);
  });
});
