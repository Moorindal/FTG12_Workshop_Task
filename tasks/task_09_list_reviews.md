# Task 09 — List Reviews (By Product + My Reviews)

## Goal
Implement endpoints to list reviews for a specific product and to list the current user's own reviews.

## Scope

### List reviews by product
- `GET /api/products/{productId}/reviews`
- Returns all **approved** reviews for the given product (non-admins see only approved).
- Paginated: `page`, `pageSize` query parameters.
- Response (200):
  ```json
  {
    "items": [
      {
        "id": 5,
        "productId": 1,
        "productName": "Samsung RF28R7351SR Refrigerator",
        "userId": 2,
        "username": "User1",
        "statusId": 2,
        "statusName": "Approved",
        "rating": 4,
        "text": "Great product.",
        "createdAt": "2026-03-01T12:00:00Z"
      }
    ],
    "page": 1,
    "pageSize": 10,
    "totalCount": 3,
    "totalPages": 1
  }
  ```
- Returns 404 if product does not exist.
- Include the product's average rating in the response (optional but useful):
  ```json
  {
    "productId": 1,
    "productName": "Samsung RF28R7351SR Refrigerator",
    "averageRating": 4.2,
    "reviews": { ... }
  }
  ```

### My reviews
- `GET /api/reviews/my`
- Returns all reviews by the current user (all statuses — the user can see their own pending/rejected reviews).
- Paginated: `page`, `pageSize` query parameters.
- Each item includes `productName` and `statusName` for display.

### MediatR requests
- `GetReviewsByProductQuery` → `GetReviewsByProductQueryHandler`.
- `GetMyReviewsQuery` → `GetMyReviewsQueryHandler`.
- Both in `Application/Reviews/Queries/`.

### Repository methods
- `IReviewRepository.GetByProductAsync(productId, page, pageSize, approvedOnly)`.
- `IReviewRepository.GetByUserAsync(userId, page, pageSize)`.

## Acceptance Criteria
- [ ] `GET /api/products/{productId}/reviews` returns paginated approved reviews (200).
- [ ] `GET /api/products/{productId}/reviews` returns 404 if product doesn't exist.
- [ ] Reviews include `username` and `statusName`.
- [ ] `GET /api/reviews/my` returns all of the current user's reviews (all statuses).
- [ ] Both endpoints support pagination (`page`, `pageSize`).
- [ ] Both endpoints require authentication (401 without token).
- [ ] Empty results return `items: []` with `totalCount: 0`.
- [ ] `dotnet build` succeeds.

## Notes / Edge Cases
- By-product reviews should only show approved reviews to regular users. If the current user is an admin, consider showing all statuses (optional, can simplify to approved-only for everyone).
- The current user's own review for a product can be in any status — they should still see it on the product page. Consider including a separate field `currentUserReview` in the product reviews response, or handle this on the frontend.
- "My reviews" should be ordered by `CreatedAt` descending (newest first).
- Product reviews should also be ordered by `CreatedAt` descending.

## Dependencies
- Task 01 (project structure).
- Task 02 (Review entity, repositories, database).
- Task 03 (MediatR, error handling).
- Task 04 (JWT auth — endpoints require authentication).
- Task 07 (Product endpoint — product existence check).

## Testing Notes
- Unit test: `GetReviewsByProductQueryHandler` — returns only approved reviews, handles missing product.
- Unit test: `GetMyReviewsQueryHandler` — returns all user reviews regardless of status.
- Integration test: Seed reviews, fetch by product → verify only approved ones returned.
- Integration test: Fetch my reviews → verify all statuses included.
