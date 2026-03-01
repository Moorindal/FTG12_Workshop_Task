import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useMyReviews, useUpdateReview } from '../hooks/useReviews';
import { ReviewForm } from '../components/reviews/ReviewForm';
import { Pagination } from '../components/common/Pagination';
import './MyReviewsPage.css';

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

function StatusBadge({ statusName }: { statusName: string }) {
  const className = statusName.toLowerCase().includes('approved')
    ? 'badge-approved'
    : statusName.toLowerCase().includes('rejected')
      ? 'badge-rejected'
      : 'badge-pending';
  return <span className={`status-badge ${className}`}>{statusName}</span>;
}

export function MyReviewsPage() {
  const { reviews, loading, error, page, setPage, totalPages, refresh } = useMyReviews();
  const { updateReview } = useUpdateReview();
  const [editingId, setEditingId] = useState<number | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const handleUpdate = async (reviewId: number, rating: number, text: string) => {
    await updateReview(reviewId, rating, text);
    setEditingId(null);
    setSuccessMessage('Your review has been resubmitted for moderation.');
    refresh();
    setTimeout(() => setSuccessMessage(null), 3000);
  };

  return (
    <div className="my-reviews-page">
      <h1>My Reviews</h1>

      {successMessage && <p className="success-message">{successMessage}</p>}
      {loading && <p className="status-loading">Loading reviews...</p>}
      {error && <p className="status-error" role="alert">{error}</p>}

      {!loading && !error && reviews.length === 0 && (
        <p className="status-empty">You haven&apos;t written any reviews yet.</p>
      )}

      <div className="my-reviews-list">
        {reviews.map((review) => (
          <div key={review.id} className="my-review-card">
            {editingId === review.id ? (
              <ReviewForm
                existingReview={review}
                onSubmit={(rating, text) => handleUpdate(review.id, rating, text)}
                onCancel={() => setEditingId(null)}
              />
            ) : (
              <>
                <div className="my-review-header">
                  <Link to={`/products/${review.productId}`} className="product-link">
                    {review.productName}
                  </Link>
                  <StatusBadge statusName={review.statusName} />
                </div>
                <div className="my-review-meta">
                  <span className="star-rating" aria-label={`${review.rating} out of 5 stars`}>
                    {'\u2605'.repeat(review.rating)}{'\u2606'.repeat(5 - review.rating)}
                  </span>
                  <span className="review-date">{formatDate(review.createdAt)}</span>
                </div>
                <p className="review-text">{review.text}</p>
                <div className="my-review-actions">
                  <button onClick={() => setEditingId(review.id)}>Edit</button>
                </div>
              </>
            )}
          </div>
        ))}
      </div>

      <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
    </div>
  );
}
