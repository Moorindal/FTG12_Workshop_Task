# Task 17 — Frontend: Add/Edit Review + My Reviews Page

## Goal
Implement the review creation form, review editing, and the "My Reviews" page.

## Scope

### API client methods
- Add to `apiClient.ts`:
  - `createReview(productId, rating, text)` → `Review`.
  - `updateReview(id, rating, text)` → `Review`.
  - `getMyReviews(page, pageSize)` → `PaginatedResponse<Review>`.

### Custom hooks
- `src/hooks/useReviews.ts`:
  - `useCreateReview()` — exposes `{ createReview, loading, error, success }`.
  - `useUpdateReview()` — exposes `{ updateReview, loading, error, success }`.
  - `useMyReviews()` — fetches current user's reviews, exposes `{ reviews, loading, error, page, setPage, totalPages }`.

### Review form component
- `src/components/reviews/ReviewForm.tsx`:
  - Props: `productId?`, `existingReview?` (for edit mode), `onSubmit`, `onCancel`.
  - Fields:
    - Rating: 1–5 selector (stars, radio buttons, or dropdown).
    - Text: multiline textarea with character counter (max 8000).
  - Client-side validation:
    - Rating required, 1–5.
    - Text required, 1–8000 characters.
  - Submit button with loading state.
  - Error display for API errors (e.g., duplicate review → show user-friendly message).

### Add review flow
- On product details page, "Add Review" button opens the review form.
- Can be inline (expand below) or a modal/dialog.
- On success: refresh reviews list, show success message, hide form.

### Edit review flow
- "My Reviews" page shows an "Edit" button on each review.
- Clicking "Edit" navigates to the product details page with the review form pre-filled.
- Or: inline editing on the "My Reviews" page.
- On success: refresh the review data, show success message.
- Note: editing resets review status to "Pending moderation" (server-side behavior).

### My Reviews page
- `src/pages/MyReviewsPage.tsx`:
  - Lists all reviews by the current user.
  - Each review shows: product name (clickable → product details), rating, text, status, date.
  - Status badge (color-coded): green for approved, yellow for pending, red for rejected.
  - "Edit" button on each review (navigates to product page or opens inline edit).
  - Pagination.
  - Empty state: "You haven't written any reviews yet."

## Acceptance Criteria
- [ ] Review form renders with rating selector and text area.
- [ ] Client-side validation prevents submitting invalid data.
- [ ] Creating a review calls the API and shows success.
- [ ] Duplicate review attempt shows a user-friendly error.
- [ ] Editing a review pre-fills the form with existing data.
- [ ] Editing calls the API and shows success.
- [ ] "My Reviews" page lists all user reviews with status badges.
- [ ] Each review on "My Reviews" links to the product page.
- [ ] Pagination works on "My Reviews".
- [ ] `npm run build` succeeds.

## Notes / Edge Cases
- After creating a review, the "Add Review" button on the product page should disappear (user can only add one per product).
- After editing, show a note: "Your review has been resubmitted for moderation."
- The 8000-character limit should be enforced client-side with a visual counter and server-side by the API.
- Handle banned user error (403): show message "Your account has been banned. You cannot create or update reviews."
- Rating input should be accessible (keyboard navigable).

## Dependencies
- Task 14 (routing, auth context).
- Task 15 (main layout).
- Task 16 (product details page — review form is shown there).
- Backend Tasks 08, 09 (create/update review, my reviews endpoints).

## Testing Notes
- Component test: ReviewForm renders fields and validates input.
- Component test: ReviewForm submits correctly in create and edit mode.
- Component test: MyReviewsPage renders reviews with status badges.
- Component test: Error display for API failures.
- These tests will be written in Task 20.
