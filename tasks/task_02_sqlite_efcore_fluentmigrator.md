# Task 02 — In-Memory SQLite + EF Core + FluentMigrator Setup

## Goal
Set up the data access layer using in-memory SQLite with EF Core for querying and FluentMigrator for schema creation and seed data.

## Scope
- Add NuGet packages: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.Data.Sqlite`, `FluentMigrator`, `FluentMigrator.Runner`.
- Create `AppDbContext` in Infrastructure with entity configurations.
- Create domain entities in the Domain project: `User`, `Product`, `ReviewStatus`, `Review`, `BannedUser`.
- Create repository interfaces in the Domain project.
- Create repository implementations in Infrastructure.
- Configure in-memory SQLite with a singleton open connection (data resets on restart).
- Ensure `PRAGMA foreign_keys = ON` is executed on every connection.
- Create FluentMigrator migrations for all tables.
- Create FluentMigrator seed data migration.
- Register everything in `Infrastructure/DependencyInjection.cs`.
- Run migrations on application startup in `Program.cs`.

## Acceptance Criteria
- [ ] Domain entities defined: `User`, `Product`, `ReviewStatus`, `Review`, `BannedUser`.
- [ ] EF Core entity configurations enforce constraints (unique username, unique userId+productId on Reviews, FK relationships).
- [ ] FluentMigrator migrations create all five tables with correct columns, types, constraints, and indexes.
- [ ] FluentMigrator seed migration inserts:
  - Users: Admin, User1, User2, User3 (passwords hashed with BCrypt, password = username).
  - Products: "Samsung RF28R7351SR Refrigerator", "LG WM4500HBA Washing Machine", "Panasonic NN-SN68KS Microwave", "Breville BKE820XL Kettle".
  - ReviewStatuses: "Pending moderation" (1), "Approved" (2), "Rejected" (3).
  - 5–8 sample reviews across different products and users.
- [ ] In-memory SQLite connection is registered as a singleton and kept open for the app lifetime.
- [ ] `PRAGMA foreign_keys = ON` is set.
- [ ] `dotnet build` succeeds.
- [ ] Application starts and migrations run without errors (visible in console logs).
- [ ] Data resets when the application restarts.

## Notes / Edge Cases
- **In-memory SQLite lifetime**: `SqliteConnection` with `"DataSource=:memory:"` must be opened immediately and registered as singleton. If the connection closes, the database is destroyed.
- **FluentMigrator + EF Core coexistence**: FluentMigrator owns the DDL. Do NOT use `EnsureCreated()` or EF Core migrations.
- **BCrypt for seed data**: Add `BCrypt.Net-Next` NuGet package to Infrastructure. Password hashing uses `BCrypt.HashPassword(password)`.
- **SQLite CHECK constraint**: Add `CHECK (Rating >= 1 AND Rating <= 5)` on the Reviews table in the FluentMigrator migration.
- **Reviews.Text max length**: 8000 characters — set `VARCHAR(8000)` in migration and configure in EF Core.
- **BannedUsers unique constraint**: `UserId` column has a UNIQUE index to enforce one active ban per user.
- **Admin user**: `IsAdministrator = true` for the Admin seed user, `false` for others.

### Entity definitions

**User**: `Id` (int PK auto), `Username` (string unique not null), `PasswordHash` (string not null), `IsAdministrator` (bool not null default false), `CreatedAt` (DateTime not null).

**Product**: `Id` (int PK auto), `Name` (string not null).

**ReviewStatus**: `Id` (int PK), `Name` (string not null).

**Review**: `Id` (int PK auto), `ProductId` (int FK not null), `UserId` (int FK not null), `StatusId` (int FK not null), `Rating` (int not null, 1–5), `Text` (string max 8000 not null), `CreatedAt` (DateTime not null).

**BannedUser**: `UserId` (int FK PK unique not null), `BannedAt` (DateTime not null).

### Indexes
- `IX_Users_Username` (unique)
- `IX_Reviews_ProductId`
- `IX_Reviews_UserId`
- `IX_Reviews_StatusId`
- `IX_Reviews_CreatedAt`
- `IX_Reviews_UserId_ProductId` (unique)
- `IX_BannedUsers_UserId` (unique)

## Dependencies
- Task 01 (Clean Architecture project structure).

## Testing Notes
- Manual: Start the application, verify console shows FluentMigrator migration output, verify no errors.
- At this stage, no automated tests yet. Add a temporary test endpoint or use the debugger to verify seed data exists.
- Verify data resets by stopping and restarting the application.
