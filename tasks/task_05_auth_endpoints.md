# Task 05 — Auth Endpoints (Login, Logout, Current User)

## Goal
Implement the authentication endpoints: login (POST), logout (POST), and current user (GET) using MediatR commands and queries.

## Scope

### Login endpoint
- `POST /api/auth/login`
- Request body: `{ "username": "string", "password": "string" }`
- Handler: Look up user by username, verify password hash, generate JWT token.
- Success response (200):
  ```json
  {
    "token": "eyJhbGciOi...",
    "user": {
      "id": 1,
      "username": "Admin",
      "isAdministrator": true,
      "isBanned": false
    }
  }
  ```
- Failure response (401): `"Invalid username or password."`
- FluentValidation: username and password are required, non-empty.

### Logout endpoint
- `POST /api/auth/logout`
- Requires authentication (`[Authorize]`).
- Server-side: no-op (JWT is stateless).
- Returns 200 with `{ "message": "Logged out successfully." }`.
- Frontend is responsible for discarding the token.

### Current user endpoint
- `GET /api/auth/me`
- Requires authentication (`[Authorize]`).
- Returns current user info extracted from JWT claims + ban status from DB.
- Response (200):
  ```json
  {
    "id": 1,
    "username": "Admin",
    "isAdministrator": true,
    "isBanned": false
  }
  ```

### Controller
- Create `AuthController` in the Web/API project.
- Thin controller: only calls `_mediator.Send()`.

### MediatR requests
- `LoginCommand` → `LoginCommandHandler` (in Application/Auth/Commands/).
- `CurrentUserQuery` → `CurrentUserQueryHandler` (in Application/Auth/Queries/).

## Acceptance Criteria
- [ ] `POST /api/auth/login` with correct credentials returns 200 + JWT token + user info.
- [ ] `POST /api/auth/login` with wrong credentials returns 401.
- [ ] `POST /api/auth/login` with missing fields returns 400 with validation errors.
- [ ] `POST /api/auth/logout` with valid token returns 200.
- [ ] `POST /api/auth/logout` without token returns 401.
- [ ] `GET /api/auth/me` with valid token returns 200 + user info including ban status.
- [ ] `GET /api/auth/me` without token returns 401.
- [ ] Login response includes the `isAdministrator` flag.
- [ ] `dotnet build` succeeds.
- [ ] Manual test: login as Admin, use token to call `/api/auth/me`.

## Notes / Edge Cases
- Login should not reveal whether the username or password was wrong (generic error message).
- Username comparison should be case-insensitive.
- The `/api/auth/me` endpoint should query the database for ban status (not just rely on token claims, since a user could be banned after token issuance).
- Ensure CORS allows the frontend origin for these endpoints.

## Dependencies
- Task 01 (project structure).
- Task 02 (User entity, database, seed data).
- Task 03 (MediatR, validation pipeline, error handling).
- Task 04 (password hashing, JWT, current user service).

## Testing Notes
- Unit test `LoginCommandHandler`: mock repository, verify correct token generation for valid credentials.
- Unit test `LoginCommandHandler`: verify exception for invalid credentials.
- Unit test `LoginCommand` validator: required fields validation.
- Integration test: Full login flow via HTTP → verify token is returned and is valid.
- Integration test: Access `/api/auth/me` with and without a token.
