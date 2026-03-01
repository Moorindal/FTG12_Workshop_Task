# Task 22 — Fix Product Reviews Page: Error After Review Creation & Enable Review Editing

## Goal

Fix the error that occurs when a user reopens a product page after submitting a review, and allow users to edit their existing review directly on the same product page instead of submitting a duplicate.

## Problem

- After a user creates a review for a product, navigating back to (or refreshing) that product page results in an error.
- There is no mechanism for a user to edit a previously submitted review from the product page.

## Scope

### Backend

#### Fix product page load error
- Investigate and fix the error that occurs when loading the product page after a review has been created.
- Ensure the "one review per user per product" uniqueness rule is enforced at the API level with a clear error response (e.g., `409 Conflict`) rather than an unhandled exception.

#### Retrieve current user's review with product reviews
- Modify `GetReviewsByProductQuery` (or the reviews controller endpoint) so that, when the request includes a valid JWT, the response indicates whether the authenticated user has already reviewed the product.
- Return the current user's review (if it exists) as a distinct field (e.g., `userReview`) alongside the paginated list of reviews.

#### Update review use-case
- Ensure `UpdateReviewCommand` supports editing rating and text of an existing review.
- Validation rules:
  - Only the review author can update their review.
  - Banned users cannot create or update reviews.
  - Rating must be between 1 and 5; text must not be empty.
- Return the updated review DTO on success.

#### HTTP status codes
- `200 OK` — review updated successfully, returns the updated review DTO.
- `201 Created` — review created successfully.
- `400 Bad Request` — validation failure (invalid rating, empty text, etc.).
- `401 Unauthorized` — missing or invalid JWT.
- `403 Forbidden` — user is banned, or user is not the review author.
- `404 Not Found` — review or product does not exist.
- `409 Conflict` — user has already reviewed this product (on create attempt).

### Frontend

#### ProductDetailsPage
- Update `src/pages/ProductDetailsPage.tsx`:
  - After fetching reviews, check whether the current user has an existing review for the product.
  - If the user **has not** reviewed the product: display the "Add Review" form (current behaviour).
  - If the user **has** reviewed the product: display their review in an editable form pre-filled with the existing rating and text, with an "Update Review" button instead of "Add Review".
- Fix any render-time crash that occurs after review creation (e.g., missing null checks, stale state, or duplicate-key errors).
- After a successful create or update, refresh the review list without a full page reload.

#### ReviewForm component
- Update `src/components/reviews/ReviewForm.tsx` to support an **edit mode**:
  - Accept optional `initialRating` and `initialText` props for pre-filling existing review data.
  - Change the submit button label to "Update Review" when editing.
  - Call the appropriate API function (`createReview` or `updateReview`) based on mode.

#### API client
- Ensure `src/services/apiClient.ts` exposes an `updateReview(reviewId, data)` function if not already present.

#### Error handling
- Display user-friendly error messages for API failures (conflict, forbidden, validation).
- No unhandled exceptions or page-level crashes under any flow.

### Testing

#### Backend — Unit tests
Extend or add tests in `FTG12_ReviewsApi.Application.Tests`:
- **Create review** — succeeds for a user who has not yet reviewed the product.
- **Create duplicate review** — returns a conflict or validation error when the user already has a review for the product.
- **Load product reviews after creation** — query returns reviews without error, includes the user's review.
- **Update review** — author can update rating and text; returns updated DTO.
- **Update review — non-owner** — returns forbidden/authorization error.
- **Banned user** — cannot create or update a review.

#### Backend — Integration tests
Extend or add tests in `FTG12_ReviewsApi.Api.Tests`:
- `POST /api/reviews` with valid data → `201 Created`.
- `POST /api/reviews` duplicate for same user+product → `409 Conflict`.
- `GET /api/reviews?productId={id}` after creation → `200 OK`, response includes the user's review.
- `PUT /api/reviews/{id}` by author → `200 OK`.
- `PUT /api/reviews/{id}` by non-author → `403 Forbidden`.
- `PUT /api/reviews/{id}` by banned user → `403 Forbidden`.

#### Frontend — UI tests
Extend `ProductDetailsPage.test.tsx`:
- Product page renders without error when the user has an existing review.
- "Update Review" button is shown instead of "Add Review" when the user has a review.
- Successful update refreshes the review list.

Extend `ReviewForm.test.tsx`:
- Form renders in **add** mode with empty fields and "Add Review" button.
- Form renders in **edit** mode with pre-filled fields and "Update Review" button.
- Submitting in edit mode calls the update API.
- Validation errors are displayed in both modes.

#### All tests must pass
- All existing backend and frontend tests continue to pass after the changes.

## Acceptance Criteria

- [ ] Product page no longer throws an error after creating a review.
- [ ] A user who has already reviewed a product sees their review in editable form on the product page.
- [ ] The "Update Review" action persists changes via the API and refreshes the review list.
- [ ] Only the review author can edit their review (enforced backend and frontend).
- [ ] Banned users cannot create or update reviews.
- [ ] One review per user per product is enforced at the API level with a proper error response.
- [ ] All existing tests continue to pass.
- [ ] New unit, integration, and UI tests cover creation, duplicate prevention, editing, authorization, and the previous crash regression.

## Relevant Files

| Area | File |
|------|------|
| Backend — Create command | `backend/src/FTG12_ReviewsApi.Application/Reviews/Commands/CreateReviewCommand.cs` |
| Backend — Update command | `backend/src/FTG12_ReviewsApi.Application/Reviews/Commands/UpdateReviewCommand.cs` |
| Backend — Product reviews query | `backend/src/FTG12_ReviewsApi.Application/Reviews/Queries/GetReviewsByProductQuery.cs` |
| Backend — Reviews controller | `backend/src/FTG12_ReviewsApi/Controllers/ReviewsController.cs` |
| Backend — Review repository interface | `backend/src/FTG12_ReviewsApi.Domain/Repositories/IReviewRepository.cs` |
| Backend — Unit tests | `backend/tests/FTG12_ReviewsApi.Application.Tests/Reviews/` |
| Backend — Integration tests | `backend/tests/FTG12_ReviewsApi.Api.Tests/` |
| Frontend — Product page | `frontend/src/pages/ProductDetailsPage.tsx` |
| Frontend — Product page tests | `frontend/src/pages/ProductDetailsPage.test.tsx` |
| Frontend — Review form | `frontend/src/components/reviews/ReviewForm.tsx` |
| Frontend — Review form tests | `frontend/src/components/reviews/ReviewForm.test.tsx` |
| Frontend — Reviews hook | `frontend/src/hooks/useReviews.ts` |
| Frontend — Reviews hook tests | `frontend/src/hooks/useReviews.test.tsx` |
| Frontend — API client | `frontend/src/services/apiClient.ts` |
