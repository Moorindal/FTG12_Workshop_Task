import type { LoginRequest, LoginResponse, UserInfo } from '../types/auth';
import type { Product, PaginatedResponse } from '../types/product';
import type { Review } from '../types/review';
import type { User } from '../types/user';

const rawApiBaseUrl = (import.meta as { env?: { VITE_API_BASE_URL?: string } }).env?.VITE_API_BASE_URL;
const API_BASE_URL =
  rawApiBaseUrl && rawApiBaseUrl.trim() !== ''
    ? rawApiBaseUrl.trim()
    : 'http://localhost:7100';

function getToken(): string | null {
  return localStorage.getItem('token');
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (response.status === 401) {
    localStorage.removeItem('token');
    window.location.href = '/login';
    throw new Error('Unauthorized');
  }

  if (!response.ok) {
    const body = await response.json().catch(() => null);
    const message = (body as { detail?: string } | null)?.detail
      ?? (body as { title?: string } | null)?.title
      ?? `Request failed with status ${response.status}`;
    const error = new Error(message);
    (error as Error & { status: number }).status = response.status;
    throw error;
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

function authHeaders(): HeadersInit {
  const token = getToken();
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  return headers;
}

async function get<T>(path: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: authHeaders(),
    signal,
  });
  return handleResponse<T>(response);
}

async function post<T>(path: string, body?: unknown, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: 'POST',
    headers: authHeaders(),
    body: body !== undefined ? JSON.stringify(body) : undefined,
    signal,
  });
  return handleResponse<T>(response);
}

async function put<T>(path: string, body?: unknown, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: 'PUT',
    headers: authHeaders(),
    body: body !== undefined ? JSON.stringify(body) : undefined,
    signal,
  });
  return handleResponse<T>(response);
}

export function login(data: LoginRequest, signal?: AbortSignal): Promise<LoginResponse> {
  return post<LoginResponse>('/api/auth/login', data, signal);
}

export function logout(signal?: AbortSignal): Promise<void> {
  return post<void>('/api/auth/logout', undefined, signal);
}

export function getCurrentUser(signal?: AbortSignal): Promise<UserInfo> {
  return get<UserInfo>('/api/auth/me', signal);
}

export function getProducts(page: number, pageSize: number, signal?: AbortSignal): Promise<PaginatedResponse<Product>> {
  return get<PaginatedResponse<Product>>(`/api/products?page=${page}&pageSize=${pageSize}`, signal);
}

export function getProduct(id: number, signal?: AbortSignal): Promise<Product> {
  return get<Product>(`/api/products/${id}`, signal);
}

export function getProductReviews(productId: number, page: number, pageSize: number, signal?: AbortSignal): Promise<PaginatedResponse<Review>> {
  return get<PaginatedResponse<Review>>(`/api/products/${productId}/reviews?page=${page}&pageSize=${pageSize}`, signal);
}

export function createReview(productId: number, rating: number, text: string, signal?: AbortSignal): Promise<Review> {
  return post<Review>('/api/reviews', { productId, rating, text }, signal);
}

export function updateReview(id: number, rating: number, text: string, signal?: AbortSignal): Promise<Review> {
  return put<Review>(`/api/reviews/${id}`, { rating, text }, signal);
}

export function getMyReviews(page: number, pageSize: number, signal?: AbortSignal): Promise<PaginatedResponse<Review>> {
  return get<PaginatedResponse<Review>>(`/api/reviews/my?page=${page}&pageSize=${pageSize}`, signal);
}

export function getAdminReviews(
  page: number,
  pageSize: number,
  statusId?: number,
  dateFrom?: string,
  dateTo?: string,
  signal?: AbortSignal
): Promise<PaginatedResponse<Review>> {
  const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
  if (statusId !== undefined) params.set('statusId', String(statusId));
  if (dateFrom) params.set('dateFrom', dateFrom);
  if (dateTo) params.set('dateTo', dateTo);
  return get<PaginatedResponse<Review>>(`/api/admin/reviews?${params.toString()}`, signal);
}

export function changeReviewStatus(reviewId: number, statusId: number, signal?: AbortSignal): Promise<Review> {
  return put<Review>(`/api/admin/reviews/${reviewId}/status`, { statusId }, signal);
}

export function getUsers(signal?: AbortSignal): Promise<User[]> {
  return get<User[]>('/api/admin/users', signal);
}

export function banUser(userId: number, signal?: AbortSignal): Promise<User> {
  return post<User>(`/api/admin/users/${userId}/ban`, undefined, signal);
}

export function unbanUser(userId: number, signal?: AbortSignal): Promise<User> {
  return post<User>(`/api/admin/users/${userId}/unban`, undefined, signal);
}
