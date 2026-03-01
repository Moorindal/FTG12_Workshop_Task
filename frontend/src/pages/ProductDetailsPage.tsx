import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import type { Product } from '../types/product';
import { useProductReviews } from '../hooks/useProductReviews';
import { useCreateReview, useUpdateReview } from '../hooks/useReviews';
import { useAuth } from '../hooks/useAuth';
import { ReviewCard } from '../components/reviews/ReviewCard';
import { ReviewForm } from '../components/reviews/ReviewForm';
import { Pagination } from '../components/common/Pagination';
import * as api from '../services/apiClient';
import './ProductDetailsPage.css';

export function ProductDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const productId = Number(id);
  const { user } = useAuth();
  const { createReview } = useCreateReview();
  const { updateReview } = useUpdateReview();

  const [product, setProduct] = useState<Product | null>(null);
  const [productLoading, setProductLoading] = useState(true);
  const [productError, setProductError] = useState<string | null>(null);
  // Reset state when navigating to a different product
  useEffect(() => {
    setProduct(null);
    setProductLoading(true);
    setProductError(null);
  }, [productId]);

  const { reviews, userReview, loading: reviewsLoading, error: reviewsError, page, setPage, totalPages, refresh } =
    useProductReviews(productId);

  const [showForm, setShowForm] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const hasReviewed = userReview !== null;

  useEffect(() => {
    const controller = new AbortController();
    api.getProduct(productId, controller.signal)
      .then((p) => {
        if (!controller.signal.aborted) setProduct(p);
      })
      .catch((err: unknown) => {
        if (!controller.signal.aborted) {
          setProductError(err instanceof Error ? err.message : 'Product not found');
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setProductLoading(false);
      });
    return () => controller.abort();
  }, [productId]);

  const handleCreateReview = async (rating: number, text: string) => {
    await createReview(productId, rating, text);
    setShowForm(false);
    setSuccessMessage('Review submitted successfully!');
    refresh();
    setTimeout(() => setSuccessMessage(null), 3000);
  };

  const handleUpdateReview = async (rating: number, text: string) => {
    if (!userReview) return;
    await updateReview(userReview.id, rating, text);
    setShowForm(false);
    setSuccessMessage('Review updated successfully!');
    refresh();
    setTimeout(() => setSuccessMessage(null), 3000);
  };

  if (productLoading) return <p className="status-loading">Loading product...</p>;
  if (productError) return <p className="status-error" role="alert">{productError}</p>;
  if (!product) return <p className="status-error">Product not found.</p>;

  return (
    <div className="product-details-page">
      <Link to="/products" className="back-link">&larr; Back to Products</Link>
      <h1>{product.name}</h1>

      <section className="reviews-section">
        <div className="reviews-header">
          <h2>Reviews</h2>
          {!reviewsLoading && !hasReviewed && !showForm && (
            <button onClick={() => setShowForm(true)}>Add Review</button>
          )}
          {!reviewsLoading && hasReviewed && !showForm && (
            <button onClick={() => setShowForm(true)}>Edit Review</button>
          )}
        </div>

        {successMessage && <p className="success-message">{successMessage}</p>}

        {showForm && hasReviewed && (
          <ReviewForm
            productId={productId}
            existingReview={userReview}
            onSubmit={handleUpdateReview}
            onCancel={() => setShowForm(false)}
          />
        )}

        {showForm && !hasReviewed && (
          <ReviewForm
            productId={productId}
            onSubmit={handleCreateReview}
            onCancel={() => setShowForm(false)}
          />
        )}

        {reviewsLoading && <p className="status-loading">Loading reviews...</p>}
        {reviewsError && <p className="status-error" role="alert">{reviewsError}</p>}

        {!reviewsLoading && !reviewsError && reviews.length === 0 && (
          <p className="status-empty">No reviews yet. Be the first to review!</p>
        )}

        <div className="reviews-list">
          {reviews.map((review) => (
            <ReviewCard key={review.id} review={review} currentUserId={user?.id} />
          ))}
        </div>

        <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
      </section>
    </div>
  );
}
