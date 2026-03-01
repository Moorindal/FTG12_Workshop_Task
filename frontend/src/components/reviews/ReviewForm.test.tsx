import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ReviewForm } from './ReviewForm';
import type { Review } from '../../types/review';

describe('ReviewForm', () => {
  const defaultProps = {
    onSubmit: vi.fn().mockResolvedValue(undefined),
    onCancel: vi.fn(),
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders empty form for new review', () => {
    render(<ReviewForm {...defaultProps} />);
    expect(screen.getByLabelText('Review')).toHaveValue('');
    expect(screen.getByText('Submit Review')).toBeInTheDocument();
  });

  it('renders pre-filled form for existing review', () => {
    const existing: Review = {
      id: 1, productId: 1, productName: 'P', userId: 1, username: 'u',
      statusId: 1, statusName: 'Pending', rating: 3, text: 'Existing text',
      createdAt: '2024-01-01T00:00:00Z',
    };
    render(<ReviewForm {...defaultProps} existingReview={existing} />);
    expect(screen.getByLabelText('Review')).toHaveValue('Existing text');
    expect(screen.getByText('Update Review')).toBeInTheDocument();
  });

  it('shows validation error when no rating is selected', async () => {
    const user = userEvent.setup();
    render(<ReviewForm {...defaultProps} />);
    await user.type(screen.getByLabelText('Review'), 'Some text');
    await user.click(screen.getByText('Submit Review'));
    expect(screen.getByRole('alert')).toHaveTextContent('Please select a rating');
    expect(defaultProps.onSubmit).not.toHaveBeenCalled();
  });

  it('shows validation error when text is empty', async () => {
    const user = userEvent.setup();
    render(<ReviewForm {...defaultProps} />);
    await user.click(screen.getByLabelText('1 star'));
    await user.click(screen.getByText('Submit Review'));
    expect(screen.getByRole('alert')).toHaveTextContent('Review text is required');
    expect(defaultProps.onSubmit).not.toHaveBeenCalled();
  });

  it('calls onSubmit with rating and text on valid submission', async () => {
    const user = userEvent.setup();
    render(<ReviewForm {...defaultProps} />);
    await user.click(screen.getByLabelText('4 stars'));
    await user.type(screen.getByLabelText('Review'), 'Nice product');
    await user.click(screen.getByText('Submit Review'));
    expect(defaultProps.onSubmit).toHaveBeenCalledWith(4, 'Nice product');
  });

  it('calls onCancel when Cancel button is clicked', async () => {
    const user = userEvent.setup();
    render(<ReviewForm {...defaultProps} />);
    await user.click(screen.getByText('Cancel'));
    expect(defaultProps.onCancel).toHaveBeenCalled();
  });

  it('shows character count', () => {
    render(<ReviewForm {...defaultProps} />);
    expect(screen.getByText('0 / 8000')).toBeInTheDocument();
  });
});
