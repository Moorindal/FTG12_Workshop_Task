# Task 06 — Authorization Policies + Banned User Enforcement

## Goal
Implement role-based authorization policies and a mechanism to prevent banned users from creating or updating reviews.

## Scope

### Authorization policies
- Define roles: `"Admin"`, `"User"`.
- Register authorization policies in `Program.cs`:
  - `"AdminOnly"` — requires role `"Admin"`.
- Apply `[Authorize]` to all authenticated endpoints by default.
- Apply `[Authorize(Policy = "AdminOnly")]` or `[Authorize(Roles = "Admin")]` to admin-only endpoints.

### Banned user enforcement
- Create a MediatR pipeline behavior `BannedUserBehavior<TRequest, TResponse>` in `Application/Common/Behaviors/`.
- The behavior checks if the current user is banned before executing commands that create or update reviews.
- If banned, throw `ForbiddenException` with message: `"Your account has been banned. You cannot create or update reviews."`.
- Mark commands that should be checked with a marker interface `IBannedUserCheck` (e.g., `CreateReviewCommand`, `UpdateReviewCommand`).

### Alternative approach (simpler)
- Instead of a pipeline behavior, implement the ban check directly in the relevant handlers (`CreateReviewCommandHandler`, `UpdateReviewCommandHandler`).
- Choose whichever approach is cleaner — document the decision.

## Acceptance Criteria
- [ ] Admin-only endpoints return 403 when called by a non-admin user.
- [ ] All authenticated endpoints return 401 when called without a token.
- [ ] Banned users receive 403 with a clear message when attempting to create or update reviews.
- [ ] Banned users can still read data (list products, list reviews, view own reviews).
- [ ] Non-banned users can create and update reviews normally.
- [ ] `dotnet build` succeeds.

## Notes / Edge Cases
- A user could be banned after their JWT was issued. The ban check must query the database, not rely on token claims.
- The `BannedUsers` table uses `UserId` as the primary identifier. If a row exists for a user, they are banned.
- Admin users should not be bannable (or if they are, it's a business decision to document). Recommended: allow banning admins as a safety measure but document it.
- The ban check should be efficient — consider caching or a lightweight query.

## Dependencies
- Task 01 (project structure).
- Task 02 (BannedUser entity, database).
- Task 03 (MediatR pipeline, exceptions).
- Task 04 (JWT auth, current user service).

## Testing Notes
- Unit test: Banned user behavior/check throws `ForbiddenException` for banned user.
- Unit test: Non-banned user passes through the behavior/check.
- Integration test: Login as a banned user, attempt to create a review → 403.
- Integration test: Login as a normal user, create a review → 201.
- Integration test: Non-admin user calls admin endpoint → 403.
