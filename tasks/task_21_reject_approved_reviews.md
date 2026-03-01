# Task 21 — Enhance Review Management: Reject Approved Reviews

## Goal

Allow administrators to reject reviews that have already been approved. The Admin Reviews page must display a "Reject" button for reviews with status "Approved", and clicking it must transition the review to "Rejected" status.

## Scope

### Backend

#### Review status transition validation
- Update `ChangeReviewStatusCommand` handler to accept the `Approved → Rejected` transition (status ID `2` → `3`).
- If the transition is invalid (e.g., rejecting an already-rejected review), return `400 Bad Request` with a clear error message.
- No changes to the controller or endpoint route are needed — the existing `PUT /api/admin/reviews/{id}/status` endpoint already accepts a `statusId` in the request body.

#### Authorization
- The endpoint is already protected with `[Authorize(Roles = "Admin")]`. Verify this continues to be enforced for the new transition.

#### HTTP status codes
- `200 OK` — status changed successfully, returns the updated review DTO.
- `400 Bad Request` — invalid status transition.
- `401 Unauthorized` — missing or invalid JWT.
- `403 Forbidden` — authenticated user is not an admin.
- `404 Not Found` — review does not exist.

### Frontend

#### ReviewStatusActions component
- Update `src/components/admin/ReviewStatusActions.tsx`:
  - When a review's status is `Approved` (status ID `2`), render a **"Reject"** button.
  - On click, show a confirmation prompt before calling the API.
  - Call `changeReviewStatus(reviewId, 3)` via the existing API client function.
  - Show a loading state on the button while the request is in progress.
  - On success, call `onStatusChange` to refresh the row in the parent table.
  - On failure, display an error message.

#### AdminReviewsPage / AdminReviewsTable
- No structural changes required. The table already renders `ReviewStatusActions` per row — the new button will appear automatically once the component is updated.

#### UI state
- After a successful status change the review row must reflect the new "Rejected" status immediately, without a full page reload.

### Testing

#### Backend — Unit tests
Extend `ChangeReviewStatusCommandHandlerTests.cs`:
- **Approved → Rejected**: handler updates the review status to `3` and returns the updated DTO.
- **Rejected → Rejected**: handler rejects the request with a validation error.
- **Non-admin user**: handler/pipeline rejects the request (covered by authorization behavior).

#### Backend — Integration tests
Extend `AdminReviewEndpointTests.cs`:
- `PUT /api/admin/reviews/{id}/status` with `{ statusId: 3 }` for an approved review → `200 OK`.
- Same request by a non-admin user → `403 Forbidden`.
- Same request without authentication → `401 Unauthorized`.
- Same request for a review already in "Rejected" status → `400 Bad Request`.

#### Frontend — UI tests
Extend `ReviewStatusActions.test.tsx`:
- "Reject" button **is visible** when the review status is `Approved`.
- "Reject" button **is not visible** when the review status is `Pending moderation` or `Rejected`.
- Clicking "Reject" and confirming calls the API with `statusId: 3`.
- On API success, `onStatusChange` callback is invoked.
- On API failure, an error message is displayed and the status remains `Approved`.

Extend `AdminReviewsPage.test.tsx`:
- An approved review row displays a "Reject" action.
- After rejection, the row status updates to "Rejected".

## Acceptance Criteria

- [ ] Admin can reject an approved review from the Admin Reviews page.
- [ ] A "Reject" button is displayed only for reviews with status "Approved".
- [ ] A confirmation prompt is shown before the rejection is executed.
- [ ] Non-admin users cannot perform this action (enforced by backend authorization).
- [ ] The UI reflects the updated "Rejected" status immediately without a page reload.
- [ ] All existing tests continue to pass.
- [ ] New unit, integration, and UI tests cover the `Approved → Rejected` transition.

## Relevant Files

| Area | File |
|------|------|
| Backend — Command handler | `backend/src/FTG12_ReviewsApi.Application/Reviews/Commands/ChangeReviewStatusCommand.cs` |
| Backend — Controller | `backend/src/FTG12_ReviewsApi/Controllers/AdminReviewsController.cs` |
| Backend — Unit tests | `backend/tests/FTG12_ReviewsApi.Application.Tests/Reviews/ChangeReviewStatusCommandHandlerTests.cs` |
| Backend — Integration tests | `backend/tests/FTG12_ReviewsApi.Api.Tests/Admin/AdminReviewEndpointTests.cs` |
| Frontend — Component | `frontend/src/components/admin/ReviewStatusActions.tsx` |
| Frontend — Component tests | `frontend/src/components/admin/ReviewStatusActions.test.tsx` |
| Frontend — Page | `frontend/src/pages/AdminReviewsPage.tsx` |
| Frontend — Page tests | `frontend/src/pages/AdminReviewsPage.test.tsx` |
| Frontend — API client | `frontend/src/services/apiClient.ts` |
