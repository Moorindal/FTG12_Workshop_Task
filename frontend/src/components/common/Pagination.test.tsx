import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Pagination } from './Pagination';

describe('Pagination', () => {
  it('renders nothing when totalPages is 1', () => {
    const { container } = render(
      <Pagination page={1} totalPages={1} onPageChange={() => {}} />
    );
    expect(container.innerHTML).toBe('');
  });

  it('renders pagination controls when totalPages > 1', () => {
    render(<Pagination page={1} totalPages={3} onPageChange={() => {}} />);
    expect(screen.getByText('Previous')).toBeInTheDocument();
    expect(screen.getByText('Next')).toBeInTheDocument();
    expect(screen.getByText('Page 1 of 3')).toBeInTheDocument();
  });

  it('disables Previous button on first page', () => {
    render(<Pagination page={1} totalPages={3} onPageChange={() => {}} />);
    expect(screen.getByText('Previous')).toBeDisabled();
    expect(screen.getByText('Next')).toBeEnabled();
  });

  it('disables Next button on last page', () => {
    render(<Pagination page={3} totalPages={3} onPageChange={() => {}} />);
    expect(screen.getByText('Previous')).toBeEnabled();
    expect(screen.getByText('Next')).toBeDisabled();
  });

  it('calls onPageChange with next page', async () => {
    const user = userEvent.setup();
    const handleChange = vi.fn();
    render(<Pagination page={2} totalPages={5} onPageChange={handleChange} />);
    await user.click(screen.getByText('Next'));
    expect(handleChange).toHaveBeenCalledWith(3);
  });

  it('calls onPageChange with previous page', async () => {
    const user = userEvent.setup();
    const handleChange = vi.fn();
    render(<Pagination page={2} totalPages={5} onPageChange={handleChange} />);
    await user.click(screen.getByText('Previous'));
    expect(handleChange).toHaveBeenCalledWith(1);
  });
});
