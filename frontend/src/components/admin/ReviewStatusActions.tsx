import { useState } from 'react';
import type { Review } from '../../types/review';
import * as api from '../../services/apiClient';
import './ReviewStatusActions.css';

interface ReviewStatusActionsProps {
  review: Review;
  onStatusChange: () => void;
}

export function ReviewStatusActions({ review, onStatusChange }: ReviewStatusActionsProps) {
  const [loading, setLoading] = useState(false);

  const handleAction = async (statusId: number) => {
    const statusLabel = statusId === 2 ? 'approve' : 'reject';
    if (!confirm(`Are you sure you want to ${statusLabel} this review?`)) return;
    setLoading(true);
    try {
      await api.changeReviewStatus(review.id, statusId);
      onStatusChange();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Action failed');
    } finally {
      setLoading(false);
    }
  };

  const statusLower = review.statusName.toLowerCase();
  const isPending = statusLower.includes('pending');
  const isRejected = statusLower.includes('rejected');
  const isApproved = statusLower.includes('approved');

  if (!isPending && !isRejected && !isApproved) return null;

  return (
    <div className="review-status-actions">
      {(isPending || isRejected) && (
        <button
          className="btn-approve"
          onClick={() => handleAction(2)}
          disabled={loading}
        >
          {loading ? '...' : 'Approve'}
        </button>
      )}
      {(isPending || isApproved) && (
        <button
          className="btn-reject"
          onClick={() => handleAction(3)}
          disabled={loading}
        >
          {loading ? '...' : 'Reject'}
        </button>
      )}
    </div>
  );
}
