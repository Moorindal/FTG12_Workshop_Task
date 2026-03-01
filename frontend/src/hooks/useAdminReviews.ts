import { useState, useEffect, useCallback } from 'react';
import type { Review } from '../types/review';
import type { PaginatedResponse } from '../types/product';
import * as api from '../services/apiClient';

interface Filters {
  statusId?: number;
  dateFrom?: string;
  dateTo?: string;
}

interface UseAdminReviewsResult {
  reviews: Review[];
  loading: boolean;
  error: string | null;
  page: number;
  setPage: (page: number) => void;
  totalPages: number;
  filters: Filters;
  setFilters: (filters: Filters) => void;
  refresh: () => void;
}

export function useAdminReviews(pageSize = 10): UseAdminReviewsResult {
  const [data, setData] = useState<PaginatedResponse<Review> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [filters, setFiltersState] = useState<Filters>({});
  const [refreshCounter, setRefreshCounter] = useState(0);

  const setFilters = useCallback((f: Filters) => {
    setLoading(true);
    setError(null);
    setFiltersState(f);
    setPage(1);
  }, []);

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
    api.getAdminReviews(page, pageSize, filters.statusId, filters.dateFrom, filters.dateTo, controller.signal)
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
  }, [page, pageSize, filters, refreshCounter]);

  return {
    reviews: data?.items ?? [],
    loading,
    error,
    page,
    setPage: changePage,
    totalPages: data?.totalPages ?? 0,
    filters,
    setFilters,
    refresh,
  };
}
