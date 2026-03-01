import type { Review } from '../../types/review';
import { ReviewStatusActions } from './ReviewStatusActions';
import './AdminReviewsTable.css';

interface AdminReviewsTableProps {
  reviews: Review[];
  onStatusChange: () => void;
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

function truncate(text: string, maxLength = 100): string {
  return text.length > maxLength ? text.slice(0, maxLength) + '...' : text;
}

export function AdminReviewsTable({ reviews, onStatusChange }: AdminReviewsTableProps) {
  if (reviews.length === 0) return null;

  return (
    <div className="admin-reviews-table-wrapper">
      <table className="admin-reviews-table">
        <thead>
          <tr>
            <th>Product</th>
            <th>User</th>
            <th>Rating</th>
            <th>Status</th>
            <th>Text</th>
            <th>Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {reviews.map((review) => (
            <tr key={review.id}>
              <td>{review.productName}</td>
              <td>{review.username}</td>
              <td>{review.rating}/5</td>
              <td>
                <span className={`status-badge badge-${review.statusName.toLowerCase().includes('approved') ? 'approved' : review.statusName.toLowerCase().includes('rejected') ? 'rejected' : 'pending'}`}>
                  {review.statusName}
                </span>
              </td>
              <td title={review.text}>{truncate(review.text)}</td>
              <td>{formatDate(review.createdAt)}</td>
              <td>
                <ReviewStatusActions review={review} onStatusChange={onStatusChange} />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
