# Task 11 — Admin User Management (List, Ban, Unban)

## Goal
Implement admin-only endpoints to list users with ban status and to ban/unban users.

## Scope

### List users
- `GET /api/admin/users`
- Admin-only.
- Returns all users with ban status.
- Response (200):
  ```json
  {
    "items": [
      {
        "id": 1,
        "username": "Admin",
        "isAdministrator": true,
        "isBanned": false,
        "bannedAt": null,
        "createdAt": "2026-03-01T00:00:00Z"
      },
      {
        "id": 2,
        "username": "User1",
        "isAdministrator": false,
        "isBanned": true,
        "bannedAt": "2026-03-01T10:00:00Z",
        "createdAt": "2026-03-01T00:00:00Z"
      }
    ]
  }
  ```
- No pagination needed (small user set for training), but can be added.

### Ban user
- `POST /api/admin/users/{id}/ban`
- Admin-only.
- Creates a row in `BannedUsers` with `UserId` and `BannedAt = DateTime.UtcNow`.
- Returns 200 with updated user DTO.
- Returns 409 if user is already banned.
- Returns 404 if user not found.

### Unban user
- `POST /api/admin/users/{id}/unban`
- Admin-only.
- Removes the row from `BannedUsers` for the given user.
- Returns 200 with updated user DTO.
- Returns 404 if user not found or not currently banned.

### MediatR requests
- `GetUsersQuery` → `GetUsersQueryHandler` (Application/Users/Queries/).
- `BanUserCommand` → `BanUserCommandHandler` (Application/Users/Commands/).
- `UnbanUserCommand` → `UnbanUserCommandHandler` (Application/Users/Commands/).

### DTOs
- `UserDto` — `Id`, `Username`, `IsAdministrator`, `IsBanned`, `BannedAt`, `CreatedAt`.

### Repository
- `IBannedUserRepository.GetByUserIdAsync(userId)`.
- `IBannedUserRepository.AddAsync(bannedUser)`.
- `IBannedUserRepository.RemoveAsync(userId)`.
- `IUserRepository.GetAllWithBanStatusAsync()` — joins Users with BannedUsers.

### Controller
- `AdminUsersController` or admin section within a users controller.
- All actions: `[Authorize(Roles = "Admin")]`.

## Acceptance Criteria
- [ ] `GET /api/admin/users` returns all users with ban status (200).
- [ ] Each user includes `isBanned` and `bannedAt` fields.
- [ ] `POST /api/admin/users/{id}/ban` bans a user (200).
- [ ] `POST /api/admin/users/{id}/ban` returns 409 if already banned.
- [ ] `POST /api/admin/users/{id}/unban` unbans a user (200).
- [ ] `POST /api/admin/users/{id}/unban` returns 404 if not banned.
- [ ] All endpoints return 403 for non-admin users.
- [ ] All endpoints return 401 without token.
- [ ] `dotnet build` succeeds.

## Notes / Edge Cases
- Should an admin be able to ban themselves? Recommended: allow it but show a warning on the frontend.
- The unique constraint on `BannedUsers.UserId` prevents double-banning at the database level. Catch and convert to `ConflictException`.
- Unbanning is a hard delete from `BannedUsers`, not a soft delete. This is simpler for a training app.
- After banning, the user's existing JWT is still valid until it expires. The ban check (Task 06) queries the database on each relevant request.

## Dependencies
- Task 01 (project structure).
- Task 02 (User and BannedUser entities, database).
- Task 03 (MediatR, validation, error handling).
- Task 04 (JWT auth).
- Task 06 (authorization policies).

## Testing Notes
- Unit test: `BanUserCommandHandler` — happy path, already banned → conflict, user not found.
- Unit test: `UnbanUserCommandHandler` — happy path, not banned → not found.
- Unit test: `GetUsersQueryHandler` — returns users with correct ban status.
- Integration test: Full ban/unban flow via HTTP.
- Integration test: After banning, verify user cannot create reviews.
