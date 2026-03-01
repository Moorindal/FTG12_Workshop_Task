import { render, screen } from '@testing-library/react';
import { ReviewCard } from './ReviewCard';
import type { Review } from '../../types/review';

describe('ReviewCard', () => {
  const review: Review = {
    id: 1,
    productId: 1,
    productName: 'Product A',
    userId: 2,
    username: 'john',
    statusId: 2,
    statusName: 'Approved',
    rating: 4,
    text: 'Great product!',
    createdAt: '2024-01-15T10:00:00Z',
  };

  it('renders the review text', () => {
    render(<ReviewCard review={review} />);
    expect(screen.getByText('Great product!')).toBeInTheDocument();
  });

  it('renders the author username', () => {
    render(<ReviewCard review={review} />);
    expect(screen.getByText('john')).toBeInTheDocument();
  });

  it('renders the star rating with aria-label', () => {
    render(<ReviewCard review={review} />);
    expect(screen.getByLabelText('4 out of 5 stars')).toBeInTheDocument();
  });

  it('shows status badge when review belongs to current user', () => {
    render(<ReviewCard review={review} currentUserId={2} />);
    expect(screen.getByText('Approved')).toBeInTheDocument();
  });

  it('shows pending status badge for own pending review', () => {
    const pendingReview: Review = { ...review, statusId: 1, statusName: 'Pending moderation' };
    render(<ReviewCard review={pendingReview} currentUserId={2} />);
    expect(screen.getByText('Pending moderation')).toBeInTheDocument();
  });

  it('shows rejected status badge for own rejected review', () => {
    const rejectedReview: Review = { ...review, statusId: 3, statusName: 'Rejected' };
    render(<ReviewCard review={rejectedReview} currentUserId={2} />);
    expect(screen.getByText('Rejected')).toBeInTheDocument();
  });

  it('does not show status badge for other users reviews', () => {
    render(<ReviewCard review={review} currentUserId={99} />);
    expect(screen.queryByText('Approved')).not.toBeInTheDocument();
  });

  it('does not show status badge when no currentUserId provided', () => {
    render(<ReviewCard review={review} />);
    expect(screen.queryByText('Approved')).not.toBeInTheDocument();
  });
});
