import { useState, useEffect, useCallback } from 'react';
import type { Review } from '../types/review';
import type { PaginatedResponse } from '../types/product';
import * as api from '../services/apiClient';

interface UseCreateReviewResult {
  createReview: (productId: number, rating: number, text: string) => Promise<Review>;
  loading: boolean;
  error: string | null;
}

export function useCreateReview(): UseCreateReviewResult {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const create = useCallback(async (productId: number, rating: number, text: string) => {
    setLoading(true);
    setError(null);
    try {
      const result = await api.createReview(productId, rating, text);
      return result;
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to create review';
      setError(message);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return { createReview: create, loading, error };
}

interface UseUpdateReviewResult {
  updateReview: (id: number, rating: number, text: string) => Promise<Review>;
  loading: boolean;
  error: string | null;
}

export function useUpdateReview(): UseUpdateReviewResult {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const update = useCallback(async (id: number, rating: number, text: string) => {
    setLoading(true);
    setError(null);
    try {
      const result = await api.updateReview(id, rating, text);
      return result;
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to update review';
      setError(message);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return { updateReview: update, loading, error };
}

interface UseMyReviewsResult {
  reviews: Review[];
  loading: boolean;
  error: string | null;
  page: number;
  setPage: (page: number) => void;
  totalPages: number;
  refresh: () => void;
}

export function useMyReviews(pageSize = 10): UseMyReviewsResult {
  const [data, setData] = useState<PaginatedResponse<Review> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [refreshCounter, setRefreshCounter] = useState(0);

  const changePage = useCallback((p: number) => {
    setLoading(true);
    setError(null);
    setPage(p);
  }, []);

  const refresh = useCallback(() => {
    setLoading(true);
    setError(null);
    setRefreshCounter((c) => c + 1);
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    api.getMyReviews(page, pageSize, controller.signal)
      .then((result) => {
        if (!controller.signal.aborted) setData(result);
      })
      .catch((err: unknown) => {
        if (!controller.signal.aborted) setError(err instanceof Error ? err.message : 'Failed to load reviews');
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });
    return () => controller.abort();
  }, [page, pageSize, refreshCounter]);

  return {
    reviews: data?.items ?? [],
    loading,
    error,
    page,
    setPage: changePage,
    totalPages: data?.totalPages ?? 0,
    refresh,
  };
}
