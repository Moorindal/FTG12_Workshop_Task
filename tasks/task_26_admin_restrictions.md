# Task 26 — Admin Restrictions: Prevent Self-Ban, Admin-Ban, and Admin Reviews

## Goal

Prevent administrators from banning themselves or other administrators (hiding the ban button for admin users on the frontend and returning an error on the backend). Additionally, prevent administrators from creating or editing reviews (hiding the add/edit review buttons on the frontend and returning an error on the backend). Update all relevant tests to cover the new restrictions.

## Problem

- An administrator can currently ban themselves via `POST /api/admin/users/{id}/ban`, which could lock out the only admin account.
- An administrator can ban other administrators, which is undesirable — admins should only be able to ban regular users.
- An administrator can create and edit product reviews (`POST /api/reviews`, `PUT /api/reviews/{id}`), which is a conflict of interest — administrators moderate reviews and should not participate as reviewers.
- The frontend shows the ban button for all users (including admins) and shows the add/edit review buttons regardless of the current user's role.

## Current State

### Ban functionality
- `BanUserCommand` handler (`BanUserCommand.cs`) does **not** check whether the target user is an admin or whether the caller is banning themselves. It only checks that the user exists and is not already banned.
- `AdminUsersController.BanUserAsync` passes the request through to MediatR with no additional guards.
- `UserRow.tsx` displays a warning dialog when banning an admin but does **not** prevent the action. There is no self-ban check.

### Review creation/editing
- `CreateReviewCommand` handler does **not** check `ICurrentUserService.IsAdmin` — admins can create reviews.
- `UpdateReviewCommand` handler does **not** check `ICurrentUserService.IsAdmin` — admins can edit reviews.
- `ReviewsController` uses `[Authorize]` which allows any authenticated user, including admins.
- `ProductDetailsPage.tsx` shows "Add Review" / "Edit Review" buttons for all logged-in users without checking `user.isAdministrator`.
- `MyReviewsPage.tsx` shows the "Edit" button for each review without checking the admin role.

### Available infrastructure
- `ICurrentUserService` provides `UserId`, `IsAdmin`, and `IsAuthenticated` properties — ready to use in command handlers.
- `User` entity has `IsAdministrator` property — available for checking whether the ban target is an admin.
- `IUserRepository.GetByIdAsync` returns the full `User` entity including `IsAdministrator`.
- Frontend `useAuth` hook provides `user.isAdministrator` — available for UI checks.

## Scope

### Backend

#### Prevent admin self-ban and admin-on-admin ban

**File:** `backend/src/FTG12_ReviewsApi.Application/Users/Commands/BanUserCommand.cs`

In the `Handle` method, after verifying the user exists and before checking if already banned:

1. Check whether `request.UserId` matches `currentUserService.UserId`. If so, throw a `ForbiddenAccessException` with the message `"Administrators cannot ban themselves."`.
2. Check whether the target `user.IsAdministrator` is `true`. If so, throw a `ForbiddenAccessException` with the message `"Administrators cannot ban other administrators."`.

These checks should return HTTP 403 Forbidden via the existing exception-handling middleware.

**File:** `backend/src/FTG12_ReviewsApi/Controllers/AdminUsersController.cs`

Add `[ProducesResponseType(StatusCodes.Status403Forbidden)]` to the `BanUserAsync` method to document the new response.

#### Prevent admin from creating reviews

**File:** `backend/src/FTG12_ReviewsApi.Application/Reviews/Commands/CreateReviewCommand.cs`

In the `Handle` method, after verifying authentication and before the product existence check:

1. Check `currentUserService.IsAdmin`. If `true`, throw a `ForbiddenAccessException` with the message `"Administrators cannot create reviews."`.

#### Prevent admin from editing reviews

**File:** `backend/src/FTG12_ReviewsApi.Application/Reviews/Commands/UpdateReviewCommand.cs`

In the `Handle` method, after verifying authentication and before the review lookup:

1. Check `currentUserService.IsAdmin`. If `true`, throw a `ForbiddenAccessException` with the message `"Administrators cannot edit reviews."`.

### Frontend

#### Hide ban button for admin users

**File:** `frontend/src/components/admin/UserRow.tsx`

- Accept Current user information (e.g. via a `currentUserId` prop or by importing `useAuth`).
- **Do not render** the Ban/Restore button if the user being displayed has `isAdministrator === true`.
- **Do not render** the Ban/Restore button if the user being displayed is the current user (self-ban prevention).
- Optionally display a visual indicator (e.g., "Admin" badge) instead of the ban button for admin users.

**File:** `frontend/src/components/admin/UserManagement.tsx`

- Pass the current user ID or admin status to `UserRow` if not already available via context.

#### Hide add/edit review buttons for admin users

**File:** `frontend/src/pages/ProductDetailsPage.tsx`

- Wrap the "Add Review" and "Edit Review" buttons in a condition that also checks `!user?.isAdministrator`.
- When `user.isAdministrator` is `true`, do not render the review form or buttons.

**File:** `frontend/src/pages/MyReviewsPage.tsx`

- Wrap the "Edit" button in a condition that also checks `!user?.isAdministrator`.
- When `user.isAdministrator` is `true`, do not render the edit button.

### Testing

#### Backend — Unit tests

**File:** `backend/tests/FTG12_ReviewsApi.Application.Tests/Users/UserCommandHandlerTests.cs`

Add to `BanUserCommandHandlerTests`:
- `Handle_WhenBanningSelf_ThrowsForbidden` — set `currentUserService.UserId` to match `request.UserId`, expect `ForbiddenAccessException`.
- `Handle_WhenBanningAdmin_ThrowsForbidden` — set target user's `IsAdministrator = true`, expect `ForbiddenAccessException`.

**File:** `backend/tests/FTG12_ReviewsApi.Application.Tests/Reviews/CreateReviewCommandHandlerTests.cs`

Add:
- `Handle_WhenAdmin_ThrowsForbidden` — set `currentUserService.IsAdmin = true`, expect `ForbiddenAccessException`.

**File:** `backend/tests/FTG12_ReviewsApi.Application.Tests/Reviews/UpdateReviewCommandHandlerTests.cs`

Add:
- `Handle_WhenAdmin_ThrowsForbidden` — set `currentUserService.IsAdmin = true`, expect `ForbiddenAccessException`.

#### Backend — Integration tests

**File:** `backend/tests/FTG12_ReviewsApi.Api.Tests/Admin/AdminUserEndpointTests.cs`

Add:
- `BanUser_Self_Returns403` — Admin (id=1) tries to ban user id=1, expect 403 Forbidden.
- `BanUser_OtherAdmin_Returns403` — if applicable (currently only one admin seeded; could test by confirming the 403 response when banning user id=1 from admin's own token — effectively the same test, or seed a second admin for this test).

**File:** `backend/tests/FTG12_ReviewsApi.Api.Tests/Reviews/ReviewEndpointTests.cs`

Add:
- `CreateReview_AsAdmin_Returns403` — authenticate as Admin (id=1, role="Admin"), POST to `/api/reviews`, expect 403 Forbidden.
- `UpdateReview_AsAdmin_Returns403` — authenticate as Admin (id=1, role="Admin"), PUT to `/api/reviews/{id}`, expect 403 Forbidden.

#### Frontend tests

**File:** `frontend/src/components/admin/UserRow.test.tsx`

Add:
- `hides ban button for admin users` — render `UserRow` with a user where `isAdministrator: true`, verify the Ban button is **not** rendered.
- `hides ban button for current user (self)` — render `UserRow` where the user id matches the current user id, verify the Ban button is **not** rendered.

**File:** `frontend/src/components/admin/UserManagement.test.tsx`

Add:
- `does not show ban button for admin users` — render the table with admin users, verify no ban buttons appear for them.

**File:** `frontend/src/pages/ProductDetailsPage.test.tsx`

Add:
- `hides Add Review button for admin users` — mock `useAuth` to return a user with `isAdministrator: true`, verify the "Add Review" button is **not** rendered.

**File:** `frontend/src/pages/MyReviewsPage.test.tsx`

Add:
- `hides Edit button for admin users` — mock `useAuth` to return a user with `isAdministrator: true`, verify the "Edit" button is **not** rendered.

## Acceptance Criteria

- [ ] `POST /api/admin/users/{id}/ban` returns 403 when the admin tries to ban themselves.
- [ ] `POST /api/admin/users/{id}/ban` returns 403 when banning a user with `IsAdministrator = true`.
- [ ] `POST /api/reviews` returns 403 when the authenticated user is an admin.
- [ ] `PUT /api/reviews/{id}` returns 403 when the authenticated user is an admin.
- [ ] Frontend: Ban/Restore button is hidden for users with `isAdministrator: true`.
- [ ] Frontend: Ban/Restore button is hidden when the user being displayed is the current user.
- [ ] Frontend: "Add Review" and "Edit Review" buttons are hidden for admin users on the product details page.
- [ ] Frontend: "Edit" button is hidden for admin users on the My Reviews page.
- [ ] All new unit tests pass.
- [ ] All new integration tests pass.
- [ ] All new frontend tests pass.
- [ ] All existing tests continue to pass.
- [ ] `dotnet build` succeeds.
- [ ] `npm run build` succeeds.

## Relevant Files

| Area | File |
|------|------|
| Backend — Ban command | `backend/src/FTG12_ReviewsApi.Application/Users/Commands/BanUserCommand.cs` |
| Backend — Admin users controller | `backend/src/FTG12_ReviewsApi/Controllers/AdminUsersController.cs` |
| Backend — Create review command | `backend/src/FTG12_ReviewsApi.Application/Reviews/Commands/CreateReviewCommand.cs` |
| Backend — Update review command | `backend/src/FTG12_ReviewsApi.Application/Reviews/Commands/UpdateReviewCommand.cs` |
| Backend — Reviews controller | `backend/src/FTG12_ReviewsApi/Controllers/ReviewsController.cs` |
| Backend — Current user service | `backend/src/FTG12_ReviewsApi.Application/Common/Interfaces/ICurrentUserService.cs` |
| Backend — User entity | `backend/src/FTG12_ReviewsApi.Domain/Entities/User.cs` |
| Backend — Forbidden exception | `backend/src/FTG12_ReviewsApi.Application/Common/Exceptions/ForbiddenAccessException.cs` |
| Backend — Ban unit tests | `backend/tests/FTG12_ReviewsApi.Application.Tests/Users/UserCommandHandlerTests.cs` |
| Backend — Create review unit tests | `backend/tests/FTG12_ReviewsApi.Application.Tests/Reviews/CreateReviewCommandHandlerTests.cs` |
| Backend — Update review unit tests | `backend/tests/FTG12_ReviewsApi.Application.Tests/Reviews/UpdateReviewCommandHandlerTests.cs` |
| Backend — Admin user integration tests | `backend/tests/FTG12_ReviewsApi.Api.Tests/Admin/AdminUserEndpointTests.cs` |
| Backend — Review integration tests | `backend/tests/FTG12_ReviewsApi.Api.Tests/Reviews/ReviewEndpointTests.cs` |
| Frontend — User row component | `frontend/src/components/admin/UserRow.tsx` |
| Frontend — User management component | `frontend/src/components/admin/UserManagement.tsx` |
| Frontend — Product details page | `frontend/src/pages/ProductDetailsPage.tsx` |
| Frontend — My reviews page | `frontend/src/pages/MyReviewsPage.tsx` |
| Frontend — UserRow tests | `frontend/src/components/admin/UserRow.test.tsx` |
| Frontend — UserManagement tests | `frontend/src/components/admin/UserManagement.test.tsx` |
| Frontend — Product details tests | `frontend/src/pages/ProductDetailsPage.test.tsx` |
| Frontend — My reviews tests | `frontend/src/pages/MyReviewsPage.test.tsx` |

## Dependencies

- Task 25 (expanded seed data) — only one admin user is seeded. The self-ban test covers the admin-ban scenario as well. If a second admin is needed for testing admin-on-admin ban separately, a test-specific setup would be required.
