import { renderHook, waitFor, act } from '@testing-library/react';
import { useAdminReviews } from './useAdminReviews';
import { http, HttpResponse } from 'msw';
import { server } from '../test/server';
import { createReview, createPaginatedResponse } from '../test/factories';

describe('useAdminReviews', () => {
  it('fetches reviews on mount', async () => {
    server.use(
      http.get('http://localhost:7100/api/admin/reviews', () => {
        return HttpResponse.json(createPaginatedResponse([
          createReview({ statusName: 'Pending moderation' }),
        ]));
      }),
    );
    const { result } = renderHook(() => useAdminReviews());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.reviews).toHaveLength(1);
  });

  it('starts with empty filters', () => {
    const { result } = renderHook(() => useAdminReviews());
    expect(result.current.filters).toEqual({});
  });

  it('setFilters resets page to 1', async () => {
    const { result } = renderHook(() => useAdminReviews());
    await waitFor(() => expect(result.current.loading).toBe(false));
    act(() => {
      result.current.setFilters({ statusId: 2 });
    });
    expect(result.current.page).toBe(1);
    expect(result.current.filters.statusId).toBe(2);
  });

  it('reports error on failure', async () => {
    server.use(
      http.get('http://localhost:7100/api/admin/reviews', () => {
        return HttpResponse.json({ title: 'Server Error' }, { status: 500 });
      }),
    );
    const { result } = renderHook(() => useAdminReviews());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.error).toBeTruthy();
  });
});
