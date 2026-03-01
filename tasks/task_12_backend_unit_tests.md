# Task 12 — Backend Unit Tests (Application Layer)

## Goal
Create the backend unit test project and write comprehensive unit tests for all Application layer handlers, validators, and behaviors.

## Scope

### Test project setup
- Create `tests/FTG12_ReviewsApi.Application.Tests/` xUnit project.
- Add NuGet packages: `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `NSubstitute`, `FluentAssertions`.
- Reference `FTG12_ReviewsApi.Application` and `FTG12_ReviewsApi.Domain`.
- Add to solution file.

### Test categories

#### Auth handlers
- `LoginCommandHandlerTests`:
  - Valid credentials → returns token and user info.
  - Invalid username → throws unauthorized.
  - Invalid password → throws unauthorized.
  - Case-insensitive username lookup.
- `CurrentUserQueryHandlerTests`:
  - Authenticated user → returns user info with ban status.
  - User not found → throws not found.

#### Review handlers
- `CreateReviewCommandHandlerTests`:
  - Happy path → creates review with Pending status.
  - Duplicate review (userId + productId) → throws conflict.
  - Non-existent product → throws not found / validation error.
  - Banned user → throws forbidden.
- `UpdateReviewCommandHandlerTests`:
  - Happy path → updates review, resets status to Pending.
  - Non-owner → throws forbidden.
  - Review not found → throws not found.
  - Banned user → throws forbidden.
- `ChangeReviewStatusCommandHandlerTests`:
  - Happy path → changes status.
  - Review not found → throws not found.
  - Invalid status → validation error.
- `GetReviewsByProductQueryHandlerTests`:
  - Returns approved reviews for product.
  - Product not found → throws not found.
  - Empty result → returns empty list.
- `GetMyReviewsQueryHandlerTests`:
  - Returns all reviews for current user (all statuses).
  - No reviews → returns empty list.
- `GetAllReviewsQueryHandlerTests`:
  - Returns all reviews (admin).
  - Filters by status.
  - Filters by date range.
  - Combined filters.

#### Product handlers
- `GetProductsQueryHandlerTests`:
  - Returns paginated products.
  - Correct pagination metadata.
- `GetProductByIdQueryHandlerTests`:
  - Product found → returns DTO.
  - Product not found → throws not found.

#### User management handlers
- `BanUserCommandHandlerTests`:
  - Happy path → bans user.
  - Already banned → throws conflict.
  - User not found → throws not found.
- `UnbanUserCommandHandlerTests`:
  - Happy path → unbans user.
  - Not banned → throws not found.
- `GetUsersQueryHandlerTests`:
  - Returns users with ban status.

#### Validators
- Test each FluentValidation validator:
  - `LoginCommandValidator` — required fields.
  - `CreateReviewCommandValidator` — rating range, text length, required fields.
  - `UpdateReviewCommandValidator` — rating range, text length.
  - `ChangeReviewStatusCommandValidator` — valid status IDs.
  - Pagination validators — page/pageSize ranges.

#### Behaviors
- `ValidationBehaviorTests`:
  - Passes with valid request.
  - Throws `ValidationException` with invalid request.
  - Passes when no validators registered.

## Acceptance Criteria
- [ ] Test project compiles and is part of the solution.
- [ ] All handler tests pass.
- [ ] All validator tests pass.
- [ ] `ValidationBehavior` tests pass.
- [ ] `dotnet test` from the solution root runs all tests and reports green.
- [ ] Tests use `NSubstitute` for mocking (no real database).
- [ ] Tests use `FluentAssertions` for readable assertions.
- [ ] No test relies on external resources (network, file system, real DB).

## Notes / Edge Cases
- Organize tests by feature folder matching the Application structure.
- Use descriptive test method names (e.g., `Handle_WithValidCredentials_ReturnsToken`).
- Do not add "Arrange", "Act", "Assert" comments (per C# instructions).
- Create shared test fixtures/builders for common test data setup if repetitive.

## Dependencies
- Tasks 01–11 (all backend features must be implemented to test them).
- Can be started in parallel with later tasks if handlers are already implemented.

## Testing Notes
- Run with `dotnet test --project tests/FTG12_ReviewsApi.Application.Tests/`.
- Target: 80%+ code coverage for Application layer handlers and validators.
