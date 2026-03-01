# Task 08 — Create + Update Review Endpoints

## Goal
Implement endpoints for creating a new review and updating an existing own review.

## Scope

### Create review endpoint
- `POST /api/reviews`
- Request body:
  ```json
  {
    "productId": 1,
    "rating": 4,
    "text": "Great product, works perfectly."
  }
  ```
- Default `statusId` = 1 (Pending moderation). The user does not set status.
- `userId` is taken from the JWT token (current user).
- `createdAt` is set to `DateTime.UtcNow`.
- Success response (201 + Location header):
  ```json
  {
    "id": 10,
    "productId": 1,
    "userId": 2,
    "statusId": 1,
    "statusName": "Pending moderation",
    "rating": 4,
    "text": "Great product, works perfectly.",
    "createdAt": "2026-03-01T12:00:00Z"
  }
  ```

### Update review endpoint
- `PUT /api/reviews/{id}`
- Request body:
  ```json
  {
    "rating": 5,
    "text": "Updated review text."
  }
  ```
- Only the review owner can update their review.
- Updating resets status to 1 (Pending moderation).
- Cannot change `productId` or `userId`.
- Success response (200): updated review DTO.
- 403 if user tries to update someone else's review.
- 404 if review not found.

### MediatR requests
- `CreateReviewCommand` → `CreateReviewCommandHandler`.
- `UpdateReviewCommand` → `UpdateReviewCommandHandler`.
- Both in `Application/Reviews/Commands/`.

### Validation (FluentValidation)
- `CreateReviewCommandValidator`:
  - `productId` required, must exist.
  - `rating` required, 1–5.
  - `text` required, 1–8000 characters.
- `UpdateReviewCommandValidator`:
  - `rating` required, 1–5.
  - `text` required, 1–8000 characters.

### DTOs
- `ReviewDto` — `Id`, `ProductId`, `ProductName`, `UserId`, `Username`, `StatusId`, `StatusName`, `Rating`, `Text`, `CreatedAt`.
- `CreateReviewRequest` — `ProductId`, `Rating`, `Text`.
- `UpdateReviewRequest` — `Rating`, `Text`.

### Controller
- Add to `ReviewsController` (create it if it doesn't exist).
- `[Authorize]` — requires authentication.

## Acceptance Criteria
- [ ] `POST /api/reviews` creates a review with status "Pending moderation" (201).
- [ ] `POST /api/reviews` with duplicate (userId + productId) returns 409 Conflict.
- [ ] `POST /api/reviews` with invalid rating returns 400.
- [ ] `POST /api/reviews` with text > 8000 chars returns 400.
- [ ] `POST /api/reviews` with non-existent productId returns 400 or 404.
- [ ] `POST /api/reviews` by a banned user returns 403.
- [ ] `PUT /api/reviews/{id}` updates the review and resets status to Pending (200).
- [ ] `PUT /api/reviews/{id}` by non-owner returns 403.
- [ ] `PUT /api/reviews/{id}` for non-existent review returns 404.
- [ ] `PUT /api/reviews/{id}` by a banned user returns 403.
- [ ] Requires authentication (401 without token).
- [ ] `dotnet build` succeeds.

## Notes / Edge Cases
- The unique constraint on `(UserId, ProductId)` in the database will throw an exception on duplicate. Catch the `DbUpdateException` and convert to `ConflictException`.
- When updating, the handler should verify the review belongs to the current user before updating.
- The `productId` existence check can be done in the handler (query the product repository).
- Consider whether an admin can create reviews — default: yes, admins are also users.

## Dependencies
- Task 01 (project structure).
- Task 02 (Review entity, database, repositories).
- Task 03 (MediatR, validation, error handling).
- Task 04 (JWT auth, current user service).
- Task 06 (banned user enforcement).

## Testing Notes
- Unit test: `CreateReviewCommandHandler` — happy path, duplicate check, banned user check.
- Unit test: `UpdateReviewCommandHandler` — happy path, ownership check, not found.
- Unit test: Validators — all validation rules.
- Integration test: Full create + update flow via HTTP.
- Integration test: Duplicate review → 409.
- Integration test: Banned user → 403.
