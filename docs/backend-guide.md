# Backend — Build and Run Guide

This guide describes how to build and run the ASP.NET Core 10 Web API backend.

---

## Prerequisites

| Requirement | Version | Verify |
|------------|---------|--------|
| .NET SDK | 10.0 or later | `dotnet --version` |

Download the .NET 10 SDK from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

---

## Project Structure

All backend files are in the `backend/` folder at the repository root. The solution follows **Clean Architecture**:

```
backend/
├── FTG12_ReviewsApi.slnx                  # Solution file
├── src/
│   ├── FTG12_ReviewsApi/                  # API host (controllers, middleware)
│   │   ├── Controllers/
│   │   │   ├── HealthController.cs
│   │   │   ├── AuthController.cs
│   │   │   ├── ProductsController.cs
│   │   │   ├── ReviewsController.cs
│   │   │   ├── AdminReviewsController.cs
│   │   │   └── AdminUsersController.cs
│   │   ├── Middleware/
│   │   │   └── ExceptionHandlingMiddleware.cs
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Properties/launchSettings.json
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   ├── FTG12_ReviewsApi.Application/      # CQRS handlers, validators, DTOs
│   │   ├── Auth/
│   │   ├── Products/
│   │   ├── Reviews/
│   │   ├── Users/
│   │   └── Common/
│   ├── FTG12_ReviewsApi.Domain/           # Entities & repository interfaces
│   │   ├── Entities/
│   │   └── Repositories/
│   └── FTG12_ReviewsApi.Infrastructure/   # EF Core, SQLite, repos, JWT, BCrypt
│       ├── Persistence/
│       ├── Repositories/
│       ├── Services/
│       └── Migrations/
└── tests/
    ├── FTG12_ReviewsApi.Application.Tests/  # Unit tests
    └── FTG12_ReviewsApi.Api.Tests/          # Integration tests
```

### Key Libraries

| Library | Purpose |
|---------|---------|
| MediatR | CQRS command/query dispatching |
| FluentValidation | Request validation (via pipeline behavior) |
| Entity Framework Core | ORM with SQLite provider |
| FluentMigrator | Database schema migrations and seed data |
| BCrypt.Net | Password hashing |
| Microsoft.AspNetCore.Authentication.JwtBearer | JWT Bearer authentication |

---

## Build

From the repository root:

```bash
cd backend
dotnet build
```

**Expected output:**

```
Build succeeded in X.Xs
```

---

## Run

```bash
cd backend
dotnet run --project src/FTG12_ReviewsApi
```

The API starts on **`http://localhost:7100`**.

**Expected console output:**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:7100
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

## Run Tests

```bash
cd backend
dotnet test
```

Runs both unit tests (`Application.Tests`) and integration tests (`Api.Tests`).

---

## Database

The application uses **SQLite in-memory** with a shared cache connection string:

```
DataSource=ReviewsDb;Mode=Memory;Cache=Shared
```

A singleton `SqliteConnection` is kept open for the application's lifetime to preserve the in-memory database. The schema is managed by **FluentMigrator** (not EF Core migrations):

| Migration | Description |
|-----------|-------------|
| `M001_CreateInitialSchema` | Creates `Users`, `Products`, `ReviewStatuses`, `Reviews`, `BannedUsers` tables |
| `M002_SeedData` | Seeds users, products, review statuses, and sample reviews |

Foreign keys are explicitly enabled via `PRAGMA foreign_keys = ON`.

**No external database setup is required** — the database is created and seeded automatically on application startup.

### Seeded Users

| Username | Password | Role |
|----------|----------|------|
| Admin | Admin | Administrator |
| User1 | User1 | Regular user |
| User2 | User2 | Regular user |
| User3 | User3 | Regular user |

### Seeded Products

Four sample products (appliances) are created on startup with 8 sample reviews.

---

## Authentication

The API uses **JWT Bearer** authentication with **BCrypt** password hashing.

### Login Flow

1. Client sends `POST /api/auth/login` with `{ username, password }`.
2. Server verifies credentials via BCrypt, checks ban status, generates a JWT.
3. Response: `{ token, user: { id, username, isAdministrator, isBanned } }`.
4. Client includes `Authorization: Bearer <token>` header on all subsequent requests.

### JWT Configuration (`appsettings.json`)

```json
{
  "Jwt": {
    "Secret": "FTG12-Training-Secret-Key-Min-32-Chars!!",
    "Issuer": "FTG12_ReviewsApi",
    "Audience": "FTG12_ReviewsApp",
    "ExpirationInHours": 24
  }
}
```

### Token Claims

| Claim | Value |
|-------|-------|
| `NameIdentifier` | User ID |
| `Name` | Username |
| `Role` | `"Admin"` or `"User"` |

### Authorization

- `[Authorize]` — requires a valid JWT (any authenticated user).
- `[Authorize(Roles = "Admin")]` — requires the `Admin` role.
- **Banned user enforcement** — a MediatR pipeline behavior (`BannedUserBehavior`) blocks commands marked with the `IBannedUserCheck` interface.

---

## API Endpoints

### Health

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `GET` | `/health` | None | Returns `{ status, timestamp }` |
| `GET` | `/healthz` | None | ASP.NET Core built-in health check (`Healthy`) |

### Auth

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `POST` | `/api/auth/login` | None | Authenticate and receive JWT |
| `POST` | `/api/auth/logout` | Bearer | Stateless logout (frontend discards token) |
| `GET` | `/api/auth/me` | Bearer | Get current user info including ban status |

### Products

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `GET` | `/api/products?page=&pageSize=` | Bearer | Paginated product list |
| `GET` | `/api/products/{id}` | Bearer | Single product by ID |

### Reviews

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `POST` | `/api/reviews` | Bearer | Create a review (one per user per product) |
| `PUT` | `/api/reviews/{id}` | Bearer | Update own review (resets status to Pending) |
| `GET` | `/api/reviews/my?page=&pageSize=` | Bearer | Current user's reviews (all statuses) |
| `GET` | `/api/products/{productId}/reviews?page=&pageSize=` | Bearer | Approved reviews for a product |

### Admin — Reviews

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `GET` | `/api/admin/reviews?statusId=&dateFrom=&dateTo=&page=&pageSize=` | Admin | All reviews with optional filters |
| `PUT` | `/api/admin/reviews/{id}/status` | Admin | Change review moderation status |

### Admin — Users

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `GET` | `/api/admin/users` | Admin | List all users with ban status |
| `POST` | `/api/admin/users/{id}/ban` | Admin | Ban a user |
| `POST` | `/api/admin/users/{id}/unban` | Admin | Unban a user |

### Review Statuses

| ID | Name |
|----|------|
| 1 | Pending moderation |
| 2 | Approved |
| 3 | Rejected |

---

## Middleware Pipeline

The request pipeline is configured in the following order:

1. **Database initialization** — runs FluentMigrator migrations on startup
2. **ExceptionHandlingMiddleware** — maps exceptions to RFC 7807 ProblemDetails responses
3. **CORS** — allows `http://localhost:7200` (frontend) with any header/method
4. **Authentication** — JWT Bearer validation
5. **Authorization** — role-based policy enforcement
6. **Controllers** — endpoint routing
7. **Health checks** — `/healthz`

---

## Configuration

### Application URLs

Configured in `Properties/launchSettings.json`. Default: **`http://localhost:7100`**.

### CORS

The backend allows cross-origin requests from `http://localhost:7200` (the frontend dev server). Configured in `Program.cs`.

### Logging

| Environment | Default Level | ASP.NET Level |
|------------|---------------|---------------|
| Production | Information | Warning |
| Development | Debug | Information |

---

## Stopping the Server

Press `Ctrl+C` in the terminal where the server is running.
