import { render, screen } from '@testing-library/react';
import { AdminReviewsTable } from './AdminReviewsTable';
import type { Review } from '../../types/review';

vi.mock('../../services/apiClient', () => ({
  changeReviewStatus: vi.fn(),
}));

describe('AdminReviewsTable', () => {
  const reviews: Review[] = [
    {
      id: 1, productId: 1, productName: 'Widget', userId: 2, username: 'user1',
      statusId: 1, statusName: 'Pending moderation', rating: 3,
      text: 'Decent product', createdAt: '2024-06-01T00:00:00Z',
    },
    {
      id: 2, productId: 2, productName: 'Gadget', userId: 3, username: 'user2',
      statusId: 2, statusName: 'Approved', rating: 5,
      text: 'Excellent!', createdAt: '2024-06-02T00:00:00Z',
    },
  ];

  it('renders table headers', () => {
    render(<AdminReviewsTable reviews={reviews} onStatusChange={() => {}} />);
    expect(screen.getByText('Product')).toBeInTheDocument();
    expect(screen.getByText('User')).toBeInTheDocument();
    expect(screen.getByText('Rating')).toBeInTheDocument();
    expect(screen.getByText('Status')).toBeInTheDocument();
    expect(screen.getByText('Actions')).toBeInTheDocument();
  });

  it('renders review rows', () => {
    render(<AdminReviewsTable reviews={reviews} onStatusChange={() => {}} />);
    expect(screen.getByText('Widget')).toBeInTheDocument();
    expect(screen.getByText('user1')).toBeInTheDocument();
    expect(screen.getByText('3/5')).toBeInTheDocument();
    expect(screen.getByText('Gadget')).toBeInTheDocument();
    expect(screen.getByText('user2')).toBeInTheDocument();
  });

  it('renders nothing when reviews array is empty', () => {
    const { container } = render(
      <AdminReviewsTable reviews={[]} onStatusChange={() => {}} />
    );
    expect(container.innerHTML).toBe('');
  });

  it('shows status badges', () => {
    render(<AdminReviewsTable reviews={reviews} onStatusChange={() => {}} />);
    expect(screen.getByText('Pending moderation')).toBeInTheDocument();
    expect(screen.getByText('Approved')).toBeInTheDocument();
  });
});
