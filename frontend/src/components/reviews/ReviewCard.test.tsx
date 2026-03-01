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
});
