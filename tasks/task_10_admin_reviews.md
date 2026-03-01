# Task 10 — Admin Reviews: Filtered List + Change Status

## Goal
Implement admin-only endpoints to list all reviews with filters and to change a review's status.

## Scope

### Admin list all reviews
- `GET /api/admin/reviews`
- Admin-only (`[Authorize(Roles = "Admin")]`).
- Returns all reviews with optional filters:
  - `statusId` (int, optional) — filter by review status.
  - `dateFrom` (DateTime, optional) — reviews created on or after this date.
  - `dateTo` (DateTime, optional) — reviews created on or before this date.
- Paginated: `page`, `pageSize` query parameters.
- Response (200): paginated list of `ReviewDto` with related names (productName, username, statusName).

### Change review status
- `PUT /api/admin/reviews/{id}/status`
- Admin-only.
- Request body:
  ```json
  {
    "statusId": 2
  }
  ```
- Allowed status transitions:
  - Pending moderation → Approved (2)
  - Pending moderation → Rejected (3)
  - Rejected → Approved (2) ("Approve rejected review")
- Success response (200): updated review DTO.
- 404 if review not found.
- 400 if `statusId` is invalid.

### MediatR requests
- `GetAllReviewsQuery` → `GetAllReviewsQueryHandler` (Application/Reviews/Queries/).
- `ChangeReviewStatusCommand` → `ChangeReviewStatusCommandHandler` (Application/Reviews/Commands/).

### Validation
- `ChangeReviewStatusCommandValidator`:
  - `statusId` must be 2 (Approved) or 3 (Rejected).
- `GetAllReviewsQueryValidator` (optional):
  - `dateFrom` <= `dateTo` if both provided.

### Controller
- Create `AdminReviewsController` or add admin actions under `ReviewsController` with `[Authorize(Roles = "Admin")]`.
- Recommended: separate `AdminController` or use route prefix `/api/admin/reviews`.

## Acceptance Criteria
- [ ] `GET /api/admin/reviews` returns all reviews (paginated) — admin only (200).
- [ ] `GET /api/admin/reviews?statusId=1` filters by Pending moderation.
- [ ] `GET /api/admin/reviews?dateFrom=2026-01-01&dateTo=2026-12-31` filters by date range.
- [ ] Filters are combinable.
- [ ] `PUT /api/admin/reviews/{id}/status` changes review status (200).
- [ ] Non-admin users receive 403 for both endpoints.
- [ ] 401 without token.
- [ ] 404 for non-existent review.
- [ ] 400 for invalid statusId.
- [ ] `dotnet build` succeeds.

## Notes / Edge Cases
- Consider whether status transitions should be restricted (e.g., cannot go from Approved back to Pending). For simplicity in a training app, allow any transition to Approved or Rejected.
- Date filters should be inclusive of the boundary dates.
- The `dateFrom`/`dateTo` should handle timezone-aware comparison (all dates stored as UTC).
- Admin reviews list should include all fields for easy admin review: productName, username, statusName, rating, text, createdAt.

## Dependencies
- Task 01 (project structure).
- Task 02 (Review entity, ReviewStatus, database).
- Task 03 (MediatR, validation, error handling).
- Task 04 (JWT auth).
- Task 05 (auth endpoints — need admin to login).
- Task 06 (authorization policies).

## Testing Notes
- Unit test: `GetAllReviewsQueryHandler` — filters work correctly (status, date range, combined).
- Unit test: `ChangeReviewStatusCommandHandler` — status change works, not found throws.
- Unit test: Validator rejects invalid status IDs.
- Integration test: Login as admin, fetch filtered reviews.
- Integration test: Login as non-admin, try admin endpoints → 403.
- Integration test: Change review status and verify it persists.
