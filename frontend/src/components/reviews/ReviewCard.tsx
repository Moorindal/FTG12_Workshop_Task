import type { Review } from '../../types/review';

interface ReviewCardProps {
  review: Review;
  currentUserId?: number;
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

function StarRating({ rating }: { rating: number }) {
  return (
    <span className="star-rating" aria-label={`${rating} out of 5 stars`}>
      {'\u2605'.repeat(rating)}{'\u2606'.repeat(5 - rating)}
    </span>
  );
}

function StatusBadge({ statusName }: { statusName: string }) {
  const className = statusName.toLowerCase().includes('approved')
    ? 'badge-approved'
    : statusName.toLowerCase().includes('rejected')
      ? 'badge-rejected'
      : 'badge-pending';
  return <span className={`status-badge ${className}`}>{statusName}</span>;
}

export function ReviewCard({ review, currentUserId }: ReviewCardProps) {
  const isOwnReview = currentUserId !== undefined && review.userId === currentUserId;

  return (
    <div className="review-card">
      <div className="review-header">
        <StarRating rating={review.rating} />
        <span className="review-author">{review.username}</span>
        <span className="review-date">{formatDate(review.createdAt)}</span>
        {isOwnReview && <StatusBadge statusName={review.statusName} />}
      </div>
      <p className="review-text">{review.text}</p>
    </div>
  );
}
