import type { Review } from '../../types/review';

interface ReviewCardProps {
  review: Review;
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

export function ReviewCard({ review }: ReviewCardProps) {
  return (
    <div className="review-card">
      <div className="review-header">
        <StarRating rating={review.rating} />
        <span className="review-author">{review.username}</span>
        <span className="review-date">{formatDate(review.createdAt)}</span>
      </div>
      <p className="review-text">{review.text}</p>
    </div>
  );
}
