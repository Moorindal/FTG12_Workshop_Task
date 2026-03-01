import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ReviewStatusActions } from './ReviewStatusActions';
import { http, HttpResponse } from 'msw';
import { server } from '../../test/server';

function makeReview(overrides = {}) {
  return {
    id: 1, productId: 1, productName: 'Widget', userId: 2, username: 'user1',
    statusId: 2, statusName: 'Approved', rating: 4, text: 'Great!',
    createdAt: '2024-06-15T10:00:00Z', ...overrides,
  };
}

describe('ReviewStatusActions', () => {
  it('renders Approve and Reject for pending reviews', () => {
    const review = makeReview({ statusId: 1, statusName: 'Pending moderation' });
    render(<ReviewStatusActions review={review} onStatusChange={vi.fn()} />);
    expect(screen.getByText('Approve')).toBeInTheDocument();
    expect(screen.getByText('Reject')).toBeInTheDocument();
  });

  it('renders only Approve for rejected reviews', () => {
    const review = makeReview({ statusId: 3, statusName: 'Rejected' });
    render(<ReviewStatusActions review={review} onStatusChange={vi.fn()} />);
    expect(screen.getByText('Approve')).toBeInTheDocument();
    expect(screen.queryByText('Reject')).not.toBeInTheDocument();
  });

  it('renders only Reject for approved reviews', () => {
    const review = makeReview({ statusId: 2, statusName: 'Approved' });
    render(<ReviewStatusActions review={review} onStatusChange={vi.fn()} />);
    expect(screen.getByText('Reject')).toBeInTheDocument();
    expect(screen.queryByText('Approve')).not.toBeInTheDocument();
  });

  it('calls API and onStatusChange on approve', async () => {
    const user = userEvent.setup();
    const onStatusChange = vi.fn();
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    const review = makeReview({ id: 42, statusId: 1, statusName: 'Pending moderation' });
    let capturedStatusId: number | null = null;
    server.use(
      http.put('http://localhost:7100/api/admin/reviews/42/status', async ({ request }) => {
        const body = await request.json() as Record<string, unknown>;
        capturedStatusId = body.statusId as number;
        return HttpResponse.json(makeReview({ id: 42, statusId: 2, statusName: 'Approved' }));
      }),
    );
    render(<ReviewStatusActions review={review} onStatusChange={onStatusChange} />);
    await user.click(screen.getByText('Approve'));
    await waitFor(() => {
      expect(onStatusChange).toHaveBeenCalled();
    });
    expect(capturedStatusId).toBe(2);
    vi.restoreAllMocks();
  });

  it('does not call API when confirmation is cancelled', async () => {
    const user = userEvent.setup();
    const onStatusChange = vi.fn();
    vi.spyOn(window, 'confirm').mockReturnValue(false);
    const review = makeReview({ statusId: 1, statusName: 'Pending moderation' });
    render(<ReviewStatusActions review={review} onStatusChange={onStatusChange} />);
    await user.click(screen.getByText('Approve'));
    expect(onStatusChange).not.toHaveBeenCalled();
    vi.restoreAllMocks();
  });

  it('calls API with statusId 3 and onStatusChange on reject for approved review', async () => {
    const user = userEvent.setup();
    const onStatusChange = vi.fn();
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    const review = makeReview({ id: 99, statusId: 2, statusName: 'Approved' });
    let capturedStatusId: number | null = null;
    server.use(
      http.put('http://localhost:7100/api/admin/reviews/99/status', async ({ request }) => {
        const body = await request.json() as Record<string, unknown>;
        capturedStatusId = body.statusId as number;
        return HttpResponse.json(makeReview({ id: 99, statusId: 3, statusName: 'Rejected' }));
      }),
    );
    render(<ReviewStatusActions review={review} onStatusChange={onStatusChange} />);
    await user.click(screen.getByText('Reject'));
    await waitFor(() => {
      expect(onStatusChange).toHaveBeenCalled();
    });
    expect(capturedStatusId).toBe(3);
    vi.restoreAllMocks();
  });
});
