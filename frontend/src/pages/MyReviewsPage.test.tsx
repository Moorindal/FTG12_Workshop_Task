import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { MyReviewsPage } from './MyReviewsPage';

const mockRefresh = vi.fn();
const mockSetPage = vi.fn();
const mockUpdateReview = vi.fn();
let mockReviewsResult = { reviews: [] as unknown[], loading: true, error: null as string | null, page: 1, setPage: mockSetPage, totalPages: 0, refresh: mockRefresh };

vi.mock('../hooks/useReviews', () => ({
  useMyReviews: () => mockReviewsResult,
  useUpdateReview: () => ({ updateReview: mockUpdateReview, loading: false, error: null }),
}));

describe('MyReviewsPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('shows loading state', () => {
    mockReviewsResult = { reviews: [], loading: true, error: null, page: 1, setPage: mockSetPage, totalPages: 0, refresh: mockRefresh };
    render(
      <MemoryRouter>
        <MyReviewsPage />
      </MemoryRouter>
    );
    expect(screen.getByText('Loading reviews...')).toBeInTheDocument();
  });

  it('renders user reviews', () => {
    mockReviewsResult = {
      reviews: [
        { id: 1, productId: 1, productName: 'Widget X', userId: 1, username: 'user1', statusId: 2, statusName: 'Approved', rating: 4, text: 'Love it!', createdAt: '2024-06-15T10:00:00Z' },
      ],
      loading: false, error: null, page: 1, setPage: mockSetPage, totalPages: 1, refresh: mockRefresh,
    };
    render(
      <MemoryRouter>
        <MyReviewsPage />
      </MemoryRouter>
    );
    expect(screen.getByText('Widget X')).toBeInTheDocument();
    expect(screen.getByText('Love it!')).toBeInTheDocument();
  });

  it('shows status badges', () => {
    mockReviewsResult = {
      reviews: [
        { id: 1, productId: 1, productName: 'P1', userId: 1, username: 'u', statusId: 2, statusName: 'Approved', rating: 4, text: 'ok', createdAt: '2024-06-15T10:00:00Z' },
        { id: 2, productId: 2, productName: 'P2', userId: 1, username: 'u', statusId: 3, statusName: 'Rejected', rating: 2, text: 'no', createdAt: '2024-06-15T10:00:00Z' },
      ],
      loading: false, error: null, page: 1, setPage: mockSetPage, totalPages: 1, refresh: mockRefresh,
    };
    render(
      <MemoryRouter>
        <MyReviewsPage />
      </MemoryRouter>
    );
    expect(screen.getByText('Approved')).toBeInTheDocument();
    expect(screen.getByText('Rejected')).toBeInTheDocument();
  });

  it('shows empty state', () => {
    mockReviewsResult = { reviews: [], loading: false, error: null, page: 1, setPage: mockSetPage, totalPages: 0, refresh: mockRefresh };
    render(
      <MemoryRouter>
        <MyReviewsPage />
      </MemoryRouter>
    );
    expect(screen.getByText(/haven.*t written any reviews/i)).toBeInTheDocument();
  });

  it('shows edit button for each review', () => {
    mockReviewsResult = {
      reviews: [
        { id: 1, productId: 1, productName: 'P1', userId: 1, username: 'u', statusId: 2, statusName: 'Approved', rating: 4, text: 'ok', createdAt: '2024-06-15T10:00:00Z' },
      ],
      loading: false, error: null, page: 1, setPage: mockSetPage, totalPages: 1, refresh: mockRefresh,
    };
    render(
      <MemoryRouter>
        <MyReviewsPage />
      </MemoryRouter>
    );
    expect(screen.getByText('Edit')).toBeInTheDocument();
  });

  it('shows error state', () => {
    mockReviewsResult = { reviews: [], loading: false, error: 'Failed to load reviews', page: 1, setPage: mockSetPage, totalPages: 0, refresh: mockRefresh };
    render(
      <MemoryRouter>
        <MyReviewsPage />
      </MemoryRouter>
    );
    expect(screen.getByRole('alert')).toBeInTheDocument();
    expect(screen.getByText('Failed to load reviews')).toBeInTheDocument();
  });
});
