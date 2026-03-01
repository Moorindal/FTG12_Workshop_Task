import { useState, type FormEvent } from 'react';
import type { Review } from '../../types/review';
import './ReviewForm.css';

interface ReviewFormProps {
  productId?: number;
  existingReview?: Review;
  onSubmit: (rating: number, text: string) => Promise<void>;
  onCancel: () => void;
}

export function ReviewForm({ existingReview, onSubmit, onCancel }: ReviewFormProps) {
  const [rating, setRating] = useState(existingReview?.rating ?? 0);
  const [text, setText] = useState(existingReview?.text ?? '');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (rating < 1 || rating > 5) {
      setError('Please select a rating between 1 and 5.');
      return;
    }
    if (!text.trim()) {
      setError('Review text is required.');
      return;
    }
    if (text.length > 8000) {
      setError('Review text must be 8000 characters or less.');
      return;
    }
    setError(null);
    setSubmitting(true);
    try {
      await onSubmit(rating, text);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to submit review';
      setError(message);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form className="review-form" onSubmit={handleSubmit} noValidate>
      <div className="form-group">
        <label>Rating</label>
        <div className="rating-selector" role="radiogroup" aria-label="Rating">
          {[1, 2, 3, 4, 5].map((value) => (
            <button
              key={value}
              type="button"
              className={`rating-star ${value <= rating ? 'selected' : ''}`}
              onClick={() => setRating(value)}
              aria-label={`${value} star${value > 1 ? 's' : ''}`}
              aria-pressed={value <= rating}
            >
              {value <= rating ? '\u2605' : '\u2606'}
            </button>
          ))}
        </div>
      </div>
      <div className="form-group">
        <label htmlFor="review-text">Review</label>
        <textarea
          id="review-text"
          value={text}
          onChange={(e) => setText(e.target.value)}
          maxLength={8000}
          rows={4}
          required
          disabled={submitting}
        />
        <span className="char-count">{text.length} / 8000</span>
      </div>
      {error && <p className="form-error" role="alert">{error}</p>}
      <div className="form-actions">
        <button type="submit" disabled={submitting}>
          {submitting ? 'Submitting...' : existingReview ? 'Update Review' : 'Submit Review'}
        </button>
        <button type="button" onClick={onCancel} disabled={submitting}>
          Cancel
        </button>
      </div>
    </form>
  );
}
