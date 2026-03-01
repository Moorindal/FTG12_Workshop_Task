import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
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

function productReviewsResponse(reviews: ReturnType<typeof createReview>[], userReview: ReturnType<typeof createReview> | null = null) {
  return { reviews: createPaginatedResponse(reviews), userReview };
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
        HttpResponse.json(productReviewsResponse([
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
        HttpResponse.json(productReviewsResponse([
          createReview({ userId: 99 }),
        ])),
      ),
    );
    renderPage(1);
    await waitFor(() => {
      expect(screen.getByText('Add Review')).toBeInTheDocument();
    });
  });

  it('shows Edit Review button when user has already reviewed', async () => {
    const userReview = createReview({ id: 10, userId: 5, username: 'TestUser', rating: 4, text: 'My review' });
    server.use(
      http.get('http://localhost:7100/api/products/1', () =>
        HttpResponse.json(createProduct({ id: 1, name: 'Widget' })),
      ),
      http.get('http://localhost:7100/api/products/1/reviews', () =>
        HttpResponse.json(productReviewsResponse([
          createReview({ userId: 99 }),
        ], userReview)),
      ),
    );
    renderPage(1);
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Widget' })).toBeInTheDocument();
    });
    await waitFor(() => {
      expect(screen.getByText('Edit Review')).toBeInTheDocument();
    });
    expect(screen.queryByText('Add Review')).not.toBeInTheDocument();
  });

  it('opens edit form with pre-filled data when Edit Review is clicked', async () => {
    const user = userEvent.setup();
    const existingReview = createReview({ id: 10, userId: 5, username: 'TestUser', rating: 3, text: 'Existing text' });
    server.use(
      http.get('http://localhost:7100/api/products/1', () =>
        HttpResponse.json(createProduct({ id: 1, name: 'Widget' })),
      ),
      http.get('http://localhost:7100/api/products/1/reviews', () =>
        HttpResponse.json(productReviewsResponse([], existingReview)),
      ),
    );
    renderPage(1);
    await waitFor(() => {
      expect(screen.getByText('Edit Review')).toBeInTheDocument();
    });
    await user.click(screen.getByText('Edit Review'));
    expect(screen.getByLabelText('Review')).toHaveValue('Existing text');
    expect(screen.getByText('Update Review')).toBeInTheDocument();
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

  it('renders own pending review in list with status badge', async () => {
    const ownPendingReview = createReview({ id: 10, userId: 5, username: 'TestUser', statusId: 1, statusName: 'Pending moderation', text: 'My pending review' });
    server.use(
      http.get('http://localhost:7100/api/products/1', () =>
        HttpResponse.json(createProduct({ id: 1, name: 'Widget' })),
      ),
      http.get('http://localhost:7100/api/products/1/reviews', () =>
        HttpResponse.json(productReviewsResponse([
          createReview({ id: 1, userId: 2, username: 'other', text: 'Other review' }),
          ownPendingReview,
        ], ownPendingReview)),
      ),
    );
    renderPage(1);
    await waitFor(() => {
      expect(screen.getByText('My pending review')).toBeInTheDocument();
    });
    expect(screen.getByText('Pending moderation')).toBeInTheDocument();
  });

  it('does not show status badge for other users reviews', async () => {
    server.use(
      http.get('http://localhost:7100/api/products/1', () =>
        HttpResponse.json(createProduct({ id: 1, name: 'Widget' })),
      ),
      http.get('http://localhost:7100/api/products/1/reviews', () =>
        HttpResponse.json(productReviewsResponse([
          createReview({ id: 1, userId: 2, username: 'other', statusId: 2, statusName: 'Approved', text: 'Other review' }),
        ])),
      ),
    );
    renderPage(1);
    await waitFor(() => {
      expect(screen.getByText('Other review')).toBeInTheDocument();
    });
    expect(screen.queryByText('Approved')).not.toBeInTheDocument();
  });
});
