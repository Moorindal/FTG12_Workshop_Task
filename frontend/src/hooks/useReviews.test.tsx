import { renderHook, waitFor, act } from '@testing-library/react';
import { useCreateReview, useUpdateReview, useMyReviews } from './useReviews';
import { http, HttpResponse } from 'msw';
import { server } from '../test/server';
import { createReview, createPaginatedResponse } from '../test/factories';

describe('useCreateReview', () => {
  it('creates a review successfully', async () => {
    server.use(
      http.post('http://localhost:7100/api/reviews', () => {
        return HttpResponse.json(createReview({ id: 10, rating: 5, text: 'New review' }), { status: 201 });
      }),
    );
    const { result } = renderHook(() => useCreateReview());
    expect(result.current.loading).toBe(false);

    let review: unknown;
    await act(async () => {
      review = await result.current.createReview(1, 5, 'New review');
    });
    expect((review as { id: number }).id).toBe(10);
    expect(result.current.loading).toBe(false);
  });

  it('sets error on failure', async () => {
    server.use(
      http.post('http://localhost:7100/api/reviews', () => {
        return HttpResponse.json({ detail: 'Validation failed' }, { status: 400 });
      }),
    );
    const { result } = renderHook(() => useCreateReview());
    await act(async () => {
      try { await result.current.createReview(1, 0, ''); } catch { /* expected */ }
    });
    expect(result.current.error).toBeTruthy();
  });
});

describe('useUpdateReview', () => {
  it('updates a review successfully', async () => {
    server.use(
      http.put('http://localhost:7100/api/reviews/5', () => {
        return HttpResponse.json(createReview({ id: 5, rating: 3, text: 'Updated' }));
      }),
    );
    const { result } = renderHook(() => useUpdateReview());
    let review: unknown;
    await act(async () => {
      review = await result.current.updateReview(5, 3, 'Updated');
    });
    expect((review as { id: number }).id).toBe(5);
  });
});

describe('useMyReviews', () => {
  it('fetches my reviews', async () => {
    server.use(
      http.get('http://localhost:7100/api/reviews/my', () => {
        return HttpResponse.json(createPaginatedResponse([
          createReview({ id: 1, userId: 1 }),
        ]));
      }),
    );
    const { result } = renderHook(() => useMyReviews());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.reviews).toHaveLength(1);
  });

  it('starts with loading true', () => {
    const { result } = renderHook(() => useMyReviews());
    expect(result.current.loading).toBe(true);
  });
});
