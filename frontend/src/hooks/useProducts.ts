import { useState, useEffect, useCallback } from 'react';
import type { Product, PaginatedResponse } from '../types/product';
import * as api from '../services/apiClient';

interface UseProductsResult {
  products: Product[];
  loading: boolean;
  error: string | null;
  page: number;
  setPage: (page: number) => void;
  totalPages: number;
}

export function useProducts(pageSize = 10): UseProductsResult {
  const [data, setData] = useState<PaginatedResponse<Product> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);

  const changePage = useCallback((p: number) => {
    setLoading(true);
    setError(null);
    setPage(p);
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    api.getProducts(page, pageSize, controller.signal)
      .then((result) => {
        if (!controller.signal.aborted) {
          setData(result);
        }
      })
      .catch((err: unknown) => {
        if (!controller.signal.aborted) {
          setError(err instanceof Error ? err.message : 'Failed to load products');
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });
    return () => controller.abort();
  }, [page, pageSize]);

  return {
    products: data?.items ?? [],
    loading,
    error,
    page,
    setPage: changePage,
    totalPages: data?.totalPages ?? 0,
  };
}
