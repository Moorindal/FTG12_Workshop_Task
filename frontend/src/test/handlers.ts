import { http, HttpResponse } from 'msw';
import { createUserInfo, createProduct, createReview, createUser, createPaginatedResponse, createJwt } from './factories';

const API = 'http://localhost:7100';

export const handlers = [
  http.post(API + '/api/auth/login', async () => {
    return HttpResponse.json({ token: createJwt(), user: createUserInfo() });
  }),
  http.post(API + '/api/auth/logout', () => new HttpResponse(null, { status: 204 })),
  http.get(API + '/api/auth/me', () => HttpResponse.json(createUserInfo())),
  http.get(API + '/api/products', () => {
    return HttpResponse.json(createPaginatedResponse([createProduct({ id: 1, name: 'Widget' }), createProduct({ id: 2, name: 'Gadget' })]));
  }),
  http.get(API + '/api/products/:id', ({ params }) => {
    const id = Number(params.id);
    if (id === 999) return HttpResponse.json({ title: 'Not Found', status: 404 }, { status: 404 });
    return HttpResponse.json(createProduct({ id, name: 'Widget ' + id }));
  }),
  http.get(API + '/api/products/:id/reviews', () => {
    return HttpResponse.json(createPaginatedResponse([createReview()]));
  }),
  http.post(API + '/api/reviews', async ({ request }) => {
    const body = await request.json() as Record<string, unknown>;
    return HttpResponse.json(createReview({ rating: body.rating as number, text: body.text as string }), { status: 201 });
  }),
  http.put(API + '/api/reviews/:id', async ({ request, params }) => {
    const body = await request.json() as Record<string, unknown>;
    return HttpResponse.json(createReview({ id: Number(params.id), rating: body.rating as number, text: body.text as string }));
  }),
  http.get(API + '/api/reviews/my', () => {
    return HttpResponse.json(createPaginatedResponse([createReview({ userId: 1 })]));
  }),
  http.get(API + '/api/admin/reviews', () => {
    return HttpResponse.json(createPaginatedResponse([createReview({ statusId: 1, statusName: 'Pending moderation' })]));
  }),
  http.put(API + '/api/admin/reviews/:id/status', async ({ params }) => {
    return HttpResponse.json(createReview({ id: Number(params.id), statusId: 2, statusName: 'Approved' }));
  }),
  http.get(API + '/api/admin/users', () => {
    return HttpResponse.json([createUser({ id: 1, username: 'admin', isAdministrator: true }), createUser({ id: 2, username: 'user1' })]);
  }),
  http.post(API + '/api/admin/users/:id/ban', ({ params }) => {
    return HttpResponse.json(createUser({ id: Number(params.id), isBanned: true, bannedAt: '2024-06-01T00:00:00Z' }));
  }),
  http.post(API + '/api/admin/users/:id/unban', ({ params }) => {
    return HttpResponse.json(createUser({ id: Number(params.id), isBanned: false, bannedAt: null }));
  }),
];
