# Architecture Improvements

> **Document purpose:** Describes all architectural decisions and improvements introduced for the FTG12 Reviews application.  
> **Date:** 2026-03-01

---

## 1. Clean Architecture — Project Structure

The existing single-project solution (`FTG12_ReviewsApi`) is restructured into four projects following Clean Architecture principles. Dependencies flow inward: Web → Application → Domain, and Infrastructure → Application → Domain.

### Target solution layout

```
backend/
├── FTG12_ReviewsApi.slnx
├── src/
│   ├── FTG12_ReviewsApi.Domain/
│   │   ├── FTG12_ReviewsApi.Domain.csproj
│   │   └── Entities/
│   │       ├── User.cs
│   │       ├── Product.cs
│   │       ├── Review.cs
│   │       ├── ReviewStatus.cs
│   │       └── BannedUser.cs
│   │   └── Interfaces/
│   │       ├── IUserRepository.cs
│   │       ├── IProductRepository.cs
│   │       ├── IReviewRepository.cs
│   │       └── IBannedUserRepository.cs
│   │
│   ├── FTG12_ReviewsApi.Application/
│   │   ├── FTG12_ReviewsApi.Application.csproj
│   │   ├── DependencyInjection.cs
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   └── ValidationBehavior.cs
│   │   │   ├── Exceptions/
│   │   │   │   ├── ValidationException.cs
│   │   │   │   ├── NotFoundException.cs
│   │   │   │   ├── ForbiddenException.cs
│   │   │   │   └── ConflictException.cs
│   │   │   └── Interfaces/
│   │   │       ├── IPasswordHasher.cs
│   │   │       ├── IJwtTokenService.cs
│   │   │       └── ICurrentUserService.cs
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   │   ├── LoginCommand.cs
│   │   │   │   └── LoginCommandHandler.cs
│   │   │   └── Queries/
│   │   │       ├── CurrentUserQuery.cs
│   │   │       └── CurrentUserQueryHandler.cs
│   │   ├── Reviews/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateReviewCommand.cs
│   │   │   │   ├── UpdateReviewCommand.cs
│   │   │   │   └── ChangeReviewStatusCommand.cs
│   │   │   ├── Queries/
│   │   │   │   ├── GetReviewsByProductQuery.cs
│   │   │   │   ├── GetMyReviewsQuery.cs
│   │   │   │   └── GetAllReviewsQuery.cs
│   │   │   └── DTOs/
│   │   │       └── ReviewDto.cs
│   │   ├── Products/
│   │   │   ├── Queries/
│   │   │   │   └── GetProductsQuery.cs
│   │   │   └── DTOs/
│   │   │       └── ProductDto.cs
│   │   └── Users/
│   │       ├── Commands/
│   │       │   ├── BanUserCommand.cs
│   │       │   └── UnbanUserCommand.cs
│   │       ├── Queries/
│   │       │   └── GetUsersQuery.cs
│   │       └── DTOs/
│   │           └── UserDto.cs
│   │
│   ├── FTG12_ReviewsApi.Infrastructure/
│   │   ├── FTG12_ReviewsApi.Infrastructure.csproj
│   │   ├── DependencyInjection.cs
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   ├── ProductConfiguration.cs
│   │   │   │   ├── ReviewConfiguration.cs
│   │   │   │   ├── ReviewStatusConfiguration.cs
│   │   │   │   └── BannedUserConfiguration.cs
│   │   │   └── Repositories/
│   │   │       ├── UserRepository.cs
│   │   │       ├── ProductRepository.cs
│   │   │       ├── ReviewRepository.cs
│   │   │       └── BannedUserRepository.cs
│   │   ├── Migrations/   (FluentMigrator)
│   │   │   ├── Migration_001_CreateUsersTable.cs
│   │   │   ├── Migration_002_CreateProductsTable.cs
│   │   │   ├── Migration_003_CreateReviewStatusesTable.cs
│   │   │   ├── Migration_004_CreateReviewsTable.cs
│   │   │   ├── Migration_005_CreateBannedUsersTable.cs
│   │   │   └── Migration_010_SeedData.cs
│   │   └── Services/
│   │       ├── PasswordHasher.cs
│   │       └── JwtTokenService.cs
│   │
│   └── FTG12_ReviewsApi/                 (Web/API — existing project, refactored)
│       ├── FTG12_ReviewsApi.csproj
│       ├── Program.cs                     (composition root)
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── ProductsController.cs
│       │   ├── ReviewsController.cs
│       │   └── UsersController.cs
│       ├── Middleware/
│       │   └── ExceptionHandlingMiddleware.cs
│       └── Services/
│           └── CurrentUserService.cs
│
├── tests/
│   ├── FTG12_ReviewsApi.Application.Tests/
│   │   └── FTG12_ReviewsApi.Application.Tests.csproj
│   └── FTG12_ReviewsApi.Api.Tests/
│       └── FTG12_ReviewsApi.Api.Tests.csproj
```

### Project responsibilities

| Project | Layer | Responsibility | References |
|---------|-------|---------------|------------|
| `Domain` | Domain | Entities, repository interfaces, domain rules | None |
| `Application` | Application | MediatR commands/queries, handlers, DTOs, validators, application-level interfaces | Domain |
| `Infrastructure` | Infrastructure | EF Core DbContext, repository implementations, FluentMigrator migrations, external services (hashing, JWT) | Domain, Application |
| `FTG12_ReviewsApi` | Web/API | Controllers (thin), middleware, DI composition, auth config, startup | Application, Infrastructure |

### Key rules
- Domain has zero external dependencies (no NuGet packages except pure .NET BCL).
- Application defines interfaces; Infrastructure implements them.
- Controllers never contain business logic — they only call `IMediator.Send()`.
- The Web project is the composition root: all DI registration happens in `Program.cs` by calling `AddApplication()` and `AddInfrastructure()` extension methods.

---

## 2. Mediator Pattern (MediatR)

All application use-cases are modeled as MediatR requests (Commands and Queries):

- **Commands** mutate state: `CreateReviewCommand`, `UpdateReviewCommand`, `ChangeReviewStatusCommand`, `LoginCommand`, `BanUserCommand`, `UnbanUserCommand`.
- **Queries** read state: `GetProductsQuery`, `GetReviewsByProductQuery`, `GetMyReviewsQuery`, `GetAllReviewsQuery`, `GetUsersQuery`, `CurrentUserQuery`.

### Pipeline behaviors (registered in order)

1. **`ValidationBehavior<TRequest, TResponse>`** — Runs all FluentValidation validators for the request before the handler executes. Collects all failures and throws a custom `ValidationException` with structured errors.

### Controller pattern

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateReviewCommand command)
{
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}
```

Controllers are thin: no logic, no try/catch (handled by middleware), no direct repository access.

---

## 3. Error Handling — Consistent Error Response Contract

A global `ExceptionHandlingMiddleware` catches all exceptions and maps them to RFC 9457 Problem Details responses:

| Exception Type | HTTP Status | `type` suffix |
|---------------|------------|---------------|
| `ValidationException` | 400 Bad Request | `/validation-error` |
| `NotFoundException` | 404 Not Found | `/not-found` |
| `ForbiddenException` | 403 Forbidden | `/forbidden` |
| `ConflictException` | 409 Conflict | `/conflict` |
| `UnauthorizedAccessException` | 401 Unauthorized | `/unauthorized` |
| Unhandled | 500 Internal Server Error | `/internal-error` |

Response shape:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation failures occurred.",
  "errors": {
    "Rating": ["Rating must be between 1 and 5."]
  }
}
```

All exception types are defined in `Application/Common/Exceptions/` and thrown by handlers. The middleware in the Web project translates them.

---

## 4. Authentication Design

### Algorithm
- **BCrypt** (via `BCrypt.Net-Next` NuGet package) for password hashing.
- BCrypt embeds the salt in the hash string, so no separate salt column is needed.
- The `Users` table stores only `PasswordHash` (single column).

### Token approach
- **JWT Bearer tokens** — stateless, no server-side session storage.
- Token contains claims: `sub` (userId), `unique_name` (username), `role` ("Admin" or "User").
- Token lifetime: 24 hours (training-friendly, avoids frequent re-login).
- Secret key configured in `appsettings.json` under `Jwt:Secret`.

### Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/auth/login` | Anonymous | Validates credentials, returns JWT |
| POST | `/api/auth/logout` | Authenticated | No-op on server (client discards token) |
| GET | `/api/auth/me` | Authenticated | Returns current user info from token |

### Frontend session persistence
- Token stored in `localStorage` under key `auth_token`.
- Sent on every API request via `Authorization: Bearer <token>` header.
- On app load, check localStorage for token; if present and not expired, restore session.
- On logout, remove token from localStorage and redirect to `/login`.
- Protected routes redirect to `/login` if no valid token exists.

---

## 5. Authorization Approach

### Roles
- **Admin** — Can change review statuses, view all reviews with filters, manage users (ban/unban). Has `IsAdministrator = true`.
- **User** — Can create/edit own reviews, view products and reviews, view own reviews.

### Implementation
- JWT claims include `role` claim.
- ASP.NET Core `[Authorize(Roles = "Admin")]` attribute on admin-only endpoints.
- `[Authorize]` on all authenticated endpoints.
- **Banned user check**: A custom authorization handler or middleware checks the `BannedUsers` table. If the current user is banned, the request is rejected with 403 Forbidden and a message: `"Your account has been banned. You cannot create or update reviews."` This applies to create/update review operations only.

### HTTP responses for authorization failures

| Scenario | Status | Message |
|----------|--------|---------|
| No token / invalid token | 401 | `Unauthorized` |
| Valid token, wrong role | 403 | `You do not have permission to perform this action.` |
| Banned user creating/updating review | 403 | `Your account has been banned. You cannot create or update reviews.` |

---

## 6. Data Integrity — Constraints and Indexes

### SQLite constraints

| Table | Constraint | Type |
|-------|-----------|------|
| Users | `Username` | UNIQUE |
| Reviews | `(UserId, ProductId)` | UNIQUE (one review per user per product) |
| Reviews | `Rating` | CHECK (`Rating >= 1 AND Rating <= 5`) |
| Reviews | `Text` | Max length 8000 chars (enforced at application level + migration) |
| Reviews | `ProductId` | FK → Products(Id) |
| Reviews | `UserId` | FK → Users(Id) |
| Reviews | `StatusId` | FK → ReviewStatuses(Id) |
| BannedUsers | `UserId` | UNIQUE + FK → Users(Id) |

### SQLite foreign key enforcement
SQLite does not enforce foreign keys by default. The connection must execute `PRAGMA foreign_keys = ON;` on every connection open. This is configured in the DbContext or connection setup.

### Indexes
- `IX_Reviews_ProductId` — for queries filtering by product.
- `IX_Reviews_UserId` — for "my reviews" queries.
- `IX_Reviews_StatusId` — for admin filtered queries.
- `IX_Reviews_CreatedAt` — for date range filtering.
- `IX_BannedUsers_UserId` — unique index (also serves as the constraint).

---

## 7. API Design Conventions

### Routing
- All API routes prefixed with `/api/`.
- Resource-based: `/api/products`, `/api/reviews`, `/api/users`, `/api/auth`.
- Nested where logical: `/api/products/{productId}/reviews` for reviews by product.

### Pagination
- Query parameters: `page` (1-based, default 1) and `pageSize` (default 10, max 50).
- Response wrapper for paginated endpoints:

```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 42,
  "totalPages": 5
}
```

### Filtering (admin reviews list)
- Query parameters: `statusId`, `dateFrom`, `dateTo`.
- All filters are optional and combinable.

### HTTP methods and status codes

| Operation | Method | Success | Error cases |
|-----------|--------|---------|-------------|
| List | GET | 200 | — |
| Get by ID | GET | 200 | 404 |
| Create | POST | 201 + Location header | 400 (validation), 409 (conflict) |
| Update | PUT | 200 | 400, 403, 404 |
| Delete / Ban | POST | 200 | 400, 403, 404 |

### Request/Response conventions
- Request bodies: JSON, validated via FluentValidation.
- Responses: JSON, camelCase property names (System.Text.Json defaults).
- Errors: RFC 9457 Problem Details (see section 3).
- Dates: ISO 8601 UTC format.

---

## 8. In-Memory SQLite Configuration

### Connection lifetime
In-memory SQLite databases are destroyed when the connection closes. To keep the database alive for the entire application lifetime:

1. Create a `SqliteConnection` with `"DataSource=:memory:"`.
2. Open the connection immediately.
3. Register it as a **Singleton** in DI.
4. Pass the open connection to EF Core's `UseSqlite()`.
5. Run FluentMigrator migrations against this connection on startup.
6. Data resets on every application restart (by design).

### EF Core and FluentMigrator coexistence
- FluentMigrator manages the schema (DDL) and seed data.
- EF Core is used for data access (queries, commands) only — no EF migrations.
- Both share the same in-memory SQLite connection.
- On startup: open connection → run FluentMigrator → EF Core `EnsureCreated()` is NOT called (FluentMigrator owns the schema).

---

## 9. Testing Strategy

### Backend — xUnit

#### Unit tests (`FTG12_ReviewsApi.Application.Tests`)
- Test MediatR handlers in isolation.
- Mock repositories (via `NSubstitute`) injected into handlers.
- Test validation rules (FluentValidation validators).
- Test domain rules (e.g., rating range, banned user cannot create review).
- Test DTOs mapping.

#### Integration tests (`FTG12_ReviewsApi.Api.Tests`)
- Use `WebApplicationFactory<Program>` to spin up the API in-process.
- The in-memory SQLite database is naturally test-friendly (isolated per test or shared per class).
- Test full request/response cycle: HTTP verb → controller → mediator → handler → repository → DB → response.
- Test authentication and authorization (send requests with/without JWT, with different roles).
- Test error responses (validation errors, not found, forbidden, conflict).

### Frontend — Vitest + React Testing Library

Vitest is the natural choice for a Vite-based project (same config, fast execution, Jest-compatible API).

#### Component tests
- Render components with mock data / mock API responses.
- Test loading, error, and success states.
- Test user interactions (form submission, button clicks, navigation).
- Test protected route behavior (redirect when not authenticated).

#### Hook tests
- Test custom hooks with `renderHook` from Testing Library.
- Mock API client responses.

#### API client tests
- Mock `fetch` and test request formatting, error handling, token attachment.

---

## 10. Frontend Architecture

### State management
- **Auth state**: React Context (`AuthContext`) providing `user`, `token`, `login()`, `logout()`, `isAdmin`.
- **Server state**: Direct fetch calls via a typed API client service (no external state management libraries needed for this scope). Use custom hooks per feature.
- Keep it simple — no Redux/Zustand (overkill for this training app).

### Routing structure

```
/login                          — Login page (public)
/                               — Redirect to /products (user) or /admin/reviews (admin)
/products                       — Products list (authenticated)
/products/:id                   — Product details + reviews (authenticated)
/my-reviews                     — My reviews list (authenticated)
/admin/reviews                  — Admin reviews table (admin only)
/admin/users                    — Admin users management (admin only)
```

### API client
- Single `apiClient.ts` module with typed methods for each endpoint.
- Automatically attaches JWT token from localStorage.
- Handles 401 responses by clearing token and redirecting to login.
- Base URL configurable via Vite environment variable.

### Component organization

```
src/
├── components/
│   ├── layout/
│   │   ├── MainLayout.tsx
│   │   ├── TopBar.tsx
│   │   └── ProtectedRoute.tsx
│   ├── auth/
│   │   └── LoginForm.tsx
│   ├── products/
│   │   ├── ProductList.tsx
│   │   ├── ProductCard.tsx
│   │   └── ProductDetails.tsx
│   ├── reviews/
│   │   ├── ReviewList.tsx
│   │   ├── ReviewForm.tsx
│   │   ├── ReviewCard.tsx
│   │   └── MyReviews.tsx
│   └── admin/
│       ├── AdminReviewsTable.tsx
│       ├── ReviewStatusActions.tsx
│       ├── UserManagement.tsx
│       └── UserRow.tsx
├── contexts/
│   └── AuthContext.tsx
├── hooks/
│   ├── useAuth.ts
│   ├── useProducts.ts
│   ├── useReviews.ts
│   └── useUsers.ts
├── pages/
│   ├── LoginPage.tsx
│   ├── ProductsPage.tsx
│   ├── ProductDetailsPage.tsx
│   ├── MyReviewsPage.tsx
│   ├── AdminReviewsPage.tsx
│   └── AdminUsersPage.tsx
├── services/
│   └── apiClient.ts
├── types/
│   ├── auth.ts
│   ├── product.ts
│   ├── review.ts
│   └── user.ts
├── App.tsx
├── App.css
└── main.tsx
```
