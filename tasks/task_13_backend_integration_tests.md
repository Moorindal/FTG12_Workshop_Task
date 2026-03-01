# Task 13 — Backend Integration Tests (API Endpoints)

## Goal
Create the backend integration test project and write end-to-end tests for all API endpoints using `WebApplicationFactory`.

## Scope

### Test project setup
- Create `tests/FTG12_ReviewsApi.Api.Tests/` xUnit project.
- Add NuGet packages: `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Microsoft.AspNetCore.Mvc.Testing`, `FluentAssertions`.
- Reference `FTG12_ReviewsApi` (Web/API project).
- Add to solution file.

### Test infrastructure
- Create a custom `WebApplicationFactory<Program>` that:
  - Uses the same in-memory SQLite setup (or a fresh one per test class).
  - Runs FluentMigrator migrations (schema + seed data).
  - Provides helper methods: `CreateAuthenticatedClient(username, role)` — generates a JWT and attaches it to the `HttpClient`.
- Create helper methods for common assertions (e.g., `AssertProblemDetails`).

### Test categories

#### Auth endpoints
- `POST /api/auth/login`:
  - Valid credentials → 200 + token.
  - Invalid credentials → 401.
  - Missing fields → 400.
- `GET /api/auth/me`:
  - With token → 200 + user info.
  - Without token → 401.
  - Banned user → 200 (can still check identity, ban status shown).

#### Product endpoints
- `GET /api/products`:
  - With token → 200 + seed products.
  - Without token → 401.
  - Pagination works.
- `GET /api/products/{id}`:
  - Existing → 200.
  - Non-existing → 404.

#### Review endpoints
- `POST /api/reviews`:
  - Create review → 201.
  - Duplicate → 409.
  - Invalid rating → 400.
  - Banned user → 403.
  - No token → 401.
- `PUT /api/reviews/{id}`:
  - Owner updates → 200.
  - Non-owner → 403.
  - Not found → 404.
- `GET /api/products/{productId}/reviews`:
  - Returns approved reviews → 200.
- `GET /api/reviews/my`:
  - Returns user's reviews → 200.

#### Admin review endpoints
- `GET /api/admin/reviews`:
  - Admin → 200.
  - Non-admin → 403.
  - Filters work.
- `PUT /api/admin/reviews/{id}/status`:
  - Admin changes status → 200.
  - Non-admin → 403.

#### Admin user management endpoints
- `GET /api/admin/users`:
  - Admin → 200 + user list with ban status.
  - Non-admin → 403.
- `POST /api/admin/users/{id}/ban`:
  - Admin bans user → 200.
  - Already banned → 409.
- `POST /api/admin/users/{id}/unban`:
  - Admin unbans user → 200.
  - Not banned → 404.

#### Cross-cutting
- Verify error responses follow Problem Details format.
- Verify CORS headers in responses.

## Acceptance Criteria
- [ ] Integration test project compiles and is part of the solution.
- [ ] All auth flow tests pass.
- [ ] All CRUD endpoint tests pass.
- [ ] All authorization tests pass (correct 401/403 responses).
- [ ] All validation error tests pass (correct 400 responses with Problem Details).
- [ ] `dotnet test` from the solution root runs both unit and integration tests and all pass.
- [ ] Tests are isolated — each test class gets a clean database.

## Notes / Edge Cases
- In-memory SQLite naturally resets per `WebApplicationFactory` instance, making test isolation straightforward.
- Use `IClassFixture<WebApplicationFactory>` for test classes that can share a factory, or create a new factory per class.
- Test the full request deserializaton → handler → database → response serialization pipeline.
- Verify that `Content-Type: application/problem+json` is set for error responses.

## Dependencies
- Tasks 01–11 (all backend features implemented).
- Task 12 (can run in parallel — unit tests are independent of integration tests).

## Testing Notes
- Run with `dotnet test --project tests/FTG12_ReviewsApi.Api.Tests/`.
- Use `HttpClient` methods (`GetAsync`, `PostAsJsonAsync`, `PutAsJsonAsync`) for requests.
- Deserialize responses to DTOs or `JsonDocument` for assertions.
