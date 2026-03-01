import { render, screen } from '@testing-library/react';
import { AdminReviewsPage } from './AdminReviewsPage';

const mockRefresh = vi.fn();
const mockSetPage = vi.fn();
const mockSetFilters = vi.fn();
let mockResult = {
  reviews: [] as unknown[],
  loading: true,
  error: null as string | null,
  page: 1,
  setPage: mockSetPage,
  totalPages: 0,
  filters: {} as Record<string, unknown>,
  setFilters: mockSetFilters,
  refresh: mockRefresh,
};

vi.mock('../hooks/useAdminReviews', () => ({
  useAdminReviews: () => mockResult,
}));

describe('AdminReviewsPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('shows loading state', () => {
    mockResult = { reviews: [], loading: true, error: null, page: 1, setPage: mockSetPage, totalPages: 0, filters: {}, setFilters: mockSetFilters, refresh: mockRefresh };
    render(<AdminReviewsPage />);
    expect(screen.getByText('Loading reviews...')).toBeInTheDocument();
  });

  it('renders heading', () => {
    mockResult = { reviews: [], loading: true, error: null, page: 1, setPage: mockSetPage, totalPages: 0, filters: {}, setFilters: mockSetFilters, refresh: mockRefresh };
    render(<AdminReviewsPage />);
    expect(screen.getByRole('heading', { name: 'Review Management' })).toBeInTheDocument();
  });

  it('renders filter controls', () => {
    mockResult = { reviews: [], loading: false, error: null, page: 1, setPage: mockSetPage, totalPages: 0, filters: {}, setFilters: mockSetFilters, refresh: mockRefresh };
    render(<AdminReviewsPage />);
    expect(screen.getByLabelText('Status')).toBeInTheDocument();
    expect(screen.getByLabelText('From')).toBeInTheDocument();
    expect(screen.getByLabelText('To')).toBeInTheDocument();
    expect(screen.getByText('Clear filters')).toBeInTheDocument();
  });

  it('renders reviews table when loaded', () => {
    mockResult = {
      reviews: [
        { id: 1, productId: 1, productName: 'Admin Widget', userId: 1, username: 'u', statusId: 1, statusName: 'Pending moderation', rating: 3, text: 'ok', createdAt: '2024-06-15T10:00:00Z' },
      ],
      loading: false, error: null, page: 1, setPage: mockSetPage, totalPages: 1, filters: {}, setFilters: mockSetFilters, refresh: mockRefresh,
    };
    render(<AdminReviewsPage />);
    expect(screen.getByText('Admin Widget')).toBeInTheDocument();
  });

  it('shows error state', () => {
    mockResult = { reviews: [], loading: false, error: 'Server error', page: 1, setPage: mockSetPage, totalPages: 0, filters: {}, setFilters: mockSetFilters, refresh: mockRefresh };
    render(<AdminReviewsPage />);
    expect(screen.getByRole('alert')).toBeInTheDocument();
    expect(screen.getByText('Server error')).toBeInTheDocument();
  });
});
