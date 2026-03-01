import type { UserInfo } from '../types/auth';
import type { Product, PaginatedResponse } from '../types/product';
import type { Review } from '../types/review';
import type { User } from '../types/user';

export function createUserInfo(overrides: Partial<UserInfo> = {}): UserInfo {
  return { id: 1, username: 'TestUser', isAdministrator: false, isBanned: false, ...overrides };
}

export function createAdminInfo(overrides: Partial<UserInfo> = {}): UserInfo {
  return { id: 99, username: 'Admin', isAdministrator: true, isBanned: false, ...overrides };
}

export function createProduct(overrides: Partial<Product> = {}): Product {
  return { id: 1, name: 'Test Widget', ...overrides };
}

export function createReview(overrides: Partial<Review> = {}): Review {
  return {
    id: 1, productId: 1, productName: 'Test Widget', userId: 2, username: 'user1',
    statusId: 2, statusName: 'Approved', rating: 4, text: 'Great product!',
    createdAt: '2024-06-15T10:00:00Z', ...overrides,
  };
}

export function createUser(overrides: Partial<User> = {}): User {
  return {
    id: 1, username: 'user1', isAdministrator: false,
    isBanned: false, bannedAt: null, createdAt: '2024-01-01T00:00:00Z', ...overrides,
  };
}

export function createPaginatedResponse<T>(items: T[], overrides: Partial<PaginatedResponse<T>> = {}): PaginatedResponse<T> {
  return { items, page: 1, pageSize: 10, totalCount: items.length, totalPages: 1, ...overrides };
}

export function createJwt(payload: Record<string, unknown> = {}): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify({ exp: Math.floor(Date.now() / 1000) + 3600, sub: '1', ...payload }));
  return header + '.' + body + '.fakesignature';
}
