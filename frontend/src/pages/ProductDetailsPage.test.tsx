import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { ProductDetailsPage } from './ProductDetailsPage';
import { http, HttpResponse } from 'msw';
import { server } from '../test/server';
import { createProduct, createReview, createPaginatedResponse } from '../test/factories';

const mockUseAuth = vi.fn();
vi.mock('../hooks/useAuth', () => ({
  useAuth: () => mockUseAuth(),
}));

function renderPage(productId = 1) {
  return render(
    <MemoryRouter initialEntries={[`/products/${productId}`]}>
      <Routes>
        <Route path="/products/:id" element={<ProductDetailsPage />} />
      </Routes>
    </MemoryRouter>
  );
}

describe('ProductDetailsPage', () => {
  beforeEach(() => {
    mockUseAuth.mockReturnValue({
      user: { id: 5, username: 'TestUser', isAdministrator: false, isBanned: false },
      isAdmin: false,
    });
  });

  it('renders product name and reviews', async () => {
    server.use(
      http.get('http://localhost:7100/api/products/1', () =>
        HttpResponse.json(createProduct({ id: 1, name: 'Super Widget' })),
      ),
      http.get('http://localhost:7100/api/products/1/reviews', () =>
        HttpResponse.json(createPaginatedResponse([
          createReview({ id: 1, userId: 2, username: 'reviewer1', text: 'Awesome!' }),
        ])),
      ),
    );
    renderPage(1);
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Super Widget' })).toBeInTheDocument();
    });
    await waitFor(() => {
      expect(screen.getByText('Awesome!')).toBeInTheDocument();
    });
  });

  it('shows Add Review button when user has not reviewed', async () => {
    server.use(
      http.get('http://localhost:7100/api/products/1', () =>
        HttpResponse.json(createProduct({ id: 1, name: 'Widget' })),
      ),
      http.get('http://localhost:7100/api/products/1/reviews', () =>
        HttpResponse.json(createPaginatedResponse([
          createReview({ userId: 99 }),
        ])),
      ),
    );
    renderPage(1);
    await waitFor(() => {
      expect(screen.getByText('Add Review')).toBeInTheDocument();
    });
  });

  it('hides Add Review when user already reviewed', async () => {
    server.use(
      http.get('http://localhost:7100/api/products/1', () =>
        HttpResponse.json(createProduct({ id: 1, name: 'Widget' })),
      ),
      http.get('http://localhost:7100/api/products/1/reviews', () =>
        HttpResponse.json(createPaginatedResponse([
          createReview({ userId: 5 }),
        ])),
      ),
    );
    renderPage(1);
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Widget' })).toBeInTheDocument();
    });
    await waitFor(() => {
      expect(screen.queryByText('Add Review')).not.toBeInTheDocument();
    });
  });

  it('shows error for non-existent product', async () => {
    server.use(
      http.get('http://localhost:7100/api/products/999', () =>
        HttpResponse.json({ title: 'Not Found', status: 404 }, { status: 404 }),
      ),
    );
    renderPage(999);
    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument();
    });
  });

  it('shows loading state', () => {
    renderPage(1);
    expect(screen.getByText('Loading product...')).toBeInTheDocument();
  });
});
