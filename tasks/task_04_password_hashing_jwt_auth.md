# Task 04 — Password Hashing Service + JWT Authentication Configuration

## Goal
Implement the password hashing service using BCrypt and configure JWT Bearer authentication for the API.

## Scope

### Password hashing
- Define `IPasswordHasher` interface in `Application/Common/Interfaces/`:
  - `string Hash(string password)`
  - `bool Verify(string password, string hash)`
- Implement `PasswordHasher` in `Infrastructure/Services/` using `BCrypt.Net-Next`.

### JWT configuration
- Add NuGet package: `Microsoft.AspNetCore.Authentication.JwtBearer` (to Web/API project).
- Define `ITokenService` interface in `Application/Common/Interfaces/`:
  - `string GenerateToken(User user)` — generates a JWT with claims: `sub` (userId), `unique_name` (username), `role` ("Admin" or "User").
- Implement `JwtTokenService` in `Infrastructure/Services/`.
- Configure JWT settings in `appsettings.json`:
  ```json
  "Jwt": {
    "Secret": "FTG12-Training-Secret-Key-Min-32-Chars!!",
    "Issuer": "FTG12_ReviewsApi",
    "Audience": "FTG12_ReviewsApp",
    "ExpirationInHours": 24
  }
  ```
- Register JWT Bearer authentication in `Program.cs`.
- Add `app.UseAuthentication()` before `app.UseAuthorization()` in the middleware pipeline.

### Current user service
- Define `ICurrentUserService` interface in `Application/Common/Interfaces/`:
  - `int? UserId { get; }`
  - `string? Username { get; }`
  - `bool IsAdmin { get; }`
  - `bool IsAuthenticated { get; }`
- Implement `CurrentUserService` in the Web/API project (reads from `HttpContext.User` claims).
- Register as Scoped in DI.

## Acceptance Criteria
- [ ] `IPasswordHasher` and `PasswordHasher` implemented; hashing and verification work correctly.
- [ ] `ITokenService` and `JwtTokenService` implemented; tokens contain correct claims.
- [ ] JWT is configured in `Program.cs` with validation parameters (issuer, audience, signing key, lifetime).
- [ ] `ICurrentUserService` reads user identity from the JWT claims in `HttpContext`.
- [ ] Token expiration is set to 24 hours.
- [ ] `appsettings.json` contains JWT configuration section.
- [ ] `dotnet build` succeeds.
- [ ] The seed data in Task 02 uses `IPasswordHasher.Hash()` to hash passwords (or hardcoded BCrypt hashes since the service may not be available at migration time — document the approach).

## Notes / Edge Cases
- **Seed data hashing**: FluentMigrator runs before DI is fully available. Pre-compute BCrypt hashes for seed passwords and hardcode them in the seed migration, OR use `BCrypt.Net.BCrypt.HashPassword()` directly in the migration class.
- **JWT secret key**: Must be at least 32 characters for HMAC-SHA256.
- **Token claims**: Use `ClaimTypes.NameIdentifier` for userId, `ClaimTypes.Name` for username, `ClaimTypes.Role` for role.
- **CORS**: Ensure the CORS policy allows the `Authorization` header from the frontend.
- **Development vs production**: In a real app, the JWT secret would come from a secret manager. For this training project, `appsettings.json` is acceptable.

## Dependencies
- Task 01 (project structure).
- Task 02 (User entity and Infrastructure project with BCrypt package).

## Testing Notes
- Unit test: `PasswordHasher.Hash()` produces a non-null string; `Verify()` returns true for correct password, false for wrong password.
- Unit test: `JwtTokenService.GenerateToken()` returns a valid JWT string with expected claims.
- Integration test: Send a request with a valid/invalid/expired JWT and verify authentication behavior.
