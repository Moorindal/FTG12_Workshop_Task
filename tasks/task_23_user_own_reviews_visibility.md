# Task 23 — Improve Product Reviews List: Show User's Own Reviews Regardless of Status

## Goal

Update the product reviews list so that reviews created by the currently logged-in user are always visible — regardless of their moderation status — and display the status of the user's own reviews in each review row.

## Problem

- Currently, the product reviews page only displays reviews with status "Pending moderation" from all users.
- A logged-in user cannot see their own reviews in the list if those reviews have a different status (e.g., Approved or Rejected).
- The review status is not shown in the review row, so users have no visibility into whether their review is pending, approved, or rejected.

## Scope

### Backend

#### Update `GetReviewsByProductQueryHandler`
- Modify the handler in `Application/Reviews/Queries/GetReviewsByProductQuery.cs` so that the paginated review list includes:
  - All **approved** reviews from other users (current behaviour).
  - All reviews by the **currently authenticated user**, regardless of status (`Pending moderation`, `Approved`, or `Rejected`).
- The user's own reviews should appear inline in the main paginated list (not only as the separate `UserReview` field).
- Maintain the existing `UserReview` field in `ProductReviewsDto` for backwards compatibility with the edit-review flow.
- Ordering: reviews should remain ordered by `CreatedAt` descending.

#### Repository changes (if needed)
- If the current `IReviewRepository.GetByProductIdAsync` method already returns all reviews for a product (all statuses), no repository change is needed — filtering adjustments happen in the handler.
- If filtering is done at the repository level, update the method (or add an overload) to accept an optional `userId` parameter so that the user's reviews bypass the status filter.

#### HTTP response
- The response shape (`ProductReviewsDto`) does not change — it still returns `Reviews` (paginated list) and `UserReview`.
- Reviews belonging to the current user in the paginated list must include accurate `StatusId` and `StatusName` values.

#### HTTP status codes
- No changes to existing status codes.

### Frontend

#### `ReviewCard` component
- Update `src/components/reviews/ReviewCard.tsx` to accept and display the review status when the review belongs to the currently logged-in user.
- Display a status badge (e.g., colour-coded label) next to the review when the review's `userId` matches the authenticated user's ID:
  - **Pending moderation** — yellow/orange badge.
  - **Approved** — green badge.
  - **Rejected** — red badge.
- Do **not** display the status badge for reviews by other users (those are always approved).

#### `ProductDetailsPage`
- Update `src/pages/ProductDetailsPage.tsx` if needed to pass the current user's ID to `ReviewCard` so it can determine whether to show the status badge.
- Ensure the user's own reviews (regardless of status) appear in the review list from the API response without additional client-side filtering.

#### Types / API client
- No changes to `src/types/review.ts` or `src/services/apiClient.ts` are expected — the `Review` type already includes `statusId` and `statusName`, and the API response shape is unchanged.

### Testing

#### Backend — Unit tests
Update or add tests in `FTG12_ReviewsApi.Application.Tests/Reviews/GetReviewsByProductQueryHandlerTests.cs`:

- **User's pending review appears in list** — when the authenticated user has a review with `StatusId = 1` (Pending moderation), it is included in `Reviews.Items`.
- **User's rejected review appears in list** — when the authenticated user has a review with `StatusId = 3` (Rejected), it is included in `Reviews.Items`.
- **User's approved review appears in list** — when the authenticated user has a review with `StatusId = 2` (Approved), it is included in `Reviews.Items` (existing behaviour, verify it still works).
- **Other users' pending reviews excluded** — reviews by other users with `StatusId = 1` do **not** appear in `Reviews.Items`.
- **Other users' rejected reviews excluded** — reviews by other users with `StatusId = 3` do **not** appear in `Reviews.Items`.
- **Pagination with mixed statuses** — pagination counts include the user's own non-approved reviews and exclude other users' non-approved reviews.
- **Unauthenticated request** — only approved reviews are returned (no user-specific inclusions).

#### Backend — Integration tests
Update or add tests in `FTG12_ReviewsApi.Api.Tests/Reviews/ReviewEndpointTests.cs`:

- `GET /api/products/{id}/reviews` as authenticated user with a pending review → `200 OK`, response `Reviews.Items` contains the user's pending review.
- `GET /api/products/{id}/reviews` as authenticated user with a rejected review → `200 OK`, response `Reviews.Items` contains the user's rejected review.
- `GET /api/products/{id}/reviews` as a different authenticated user → `200 OK`, response `Reviews.Items` does **not** contain the first user's pending/rejected reviews.
- `GET /api/products/{id}/reviews` without authentication → `200 OK`, response contains only approved reviews.

#### Frontend — Component tests
Update or add tests in `frontend/src/components/reviews/ReviewCard.test.tsx`:

- Status badge is **visible** when the review belongs to the current user.
- Status badge shows correct text and styling for each status (`Pending moderation`, `Approved`, `Rejected`).
- Status badge is **not visible** when the review belongs to a different user.

Update or add tests in `frontend/src/pages/ProductDetailsPage.test.tsx`:

- The user's own pending review is rendered in the review list.
- The user's own review displays a status badge.

## Acceptance Criteria

- [ ] Authenticated user's own reviews appear in the product reviews list regardless of status.
- [ ] Other users' reviews are only shown if their status is Approved.
- [ ] The user's own review displays a status badge (Pending moderation / Approved / Rejected) in the review row.
- [ ] Status badges are not shown for other users' reviews.
- [ ] Pagination correctly accounts for the inclusion of the user's own non-approved reviews.
- [ ] Unauthenticated users see only approved reviews (no regression).
- [ ] The `UserReview` field in the response continues to work for the edit-review flow.
- [ ] All existing tests continue to pass.
- [ ] New unit, integration, and frontend tests cover the updated behaviour.
- [ ] `dotnet build` succeeds.
- [ ] `npm run build` succeeds.

## Relevant Files

| Area | File |
|------|------|
| Backend — Query handler | `backend/src/FTG12_ReviewsApi.Application/Reviews/Queries/GetReviewsByProductQuery.cs` |
| Backend — Repository interface | `backend/src/FTG12_ReviewsApi.Domain/Repositories/IReviewRepository.cs` |
| Backend — Repository implementation | `backend/src/FTG12_ReviewsApi.Infrastructure/Repositories/ReviewRepository.cs` |
| Backend — Controller | `backend/src/FTG12_ReviewsApi/Controllers/ReviewsController.cs` |
| Backend — DTOs | `backend/src/FTG12_ReviewsApi.Application/Reviews/DTOs/ProductReviewsDto.cs` |
| Backend — Unit tests | `backend/tests/FTG12_ReviewsApi.Application.Tests/Reviews/GetReviewsByProductQueryHandlerTests.cs` |
| Backend — Integration tests | `backend/tests/FTG12_ReviewsApi.Api.Tests/Reviews/ReviewEndpointTests.cs` |
| Frontend — ReviewCard component | `frontend/src/components/reviews/ReviewCard.tsx` |
| Frontend — ReviewCard tests | `frontend/src/components/reviews/ReviewCard.test.tsx` |
| Frontend — ProductDetailsPage | `frontend/src/pages/ProductDetailsPage.tsx` |
| Frontend — ProductDetailsPage tests | `frontend/src/pages/ProductDetailsPage.test.tsx` |
| Frontend — Review type | `frontend/src/types/review.ts` |

## Dependencies

- Task 09 (List reviews by product + my reviews).
- Task 16 (Product details page).
- Task 22 (Product reviews page fix and edit review).

## Notes / Edge Cases

- The user's own review may appear both in `Reviews.Items` and as `UserReview`. This is intentional — `UserReview` drives the add/edit flow, while the list inclusion provides visibility.
- If the user's review changes status (e.g., admin approves it), refreshing the page should reflect the new status badge immediately.
- Consider de-duplicating the user's review in the list if it would otherwise appear twice (once from the approved filter and once from the user-specific inclusion) — this applies when the user's review is already approved.
- The `ReviewCard` status badge styling should be consistent with the status badges used on the "My Reviews" page (`MyReviewsPage.tsx`).
