import { useState, useEffect, useCallback } from 'react';
import type { Review } from '../types/review';
import type { ProductReviewsResponse } from '../services/apiClient';
import * as api from '../services/apiClient';

interface UseProductReviewsResult {
  reviews: Review[];
  userReview: Review | null;
  loading: boolean;
  error: string | null;
  page: number;
  setPage: (page: number) => void;
  totalPages: number;
  refresh: () => void;
}

export function useProductReviews(productId: number, pageSize = 10): UseProductReviewsResult {
  const [data, setData] = useState<ProductReviewsResponse | null>(null);
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
    api.getProductReviews(productId, page, pageSize, controller.signal)
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
  }, [productId, page, pageSize, refreshCounter]);

  return {
    reviews: data?.reviews.items ?? [],
    userReview: data?.userReview ?? null,
    loading,
    error,
    page,
    setPage: changePage,
    totalPages: data?.reviews.totalPages ?? 0,
    refresh,
  };
}
