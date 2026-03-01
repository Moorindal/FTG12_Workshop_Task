# Task 20 — Frontend Tests (Vitest + React Testing Library)

## Goal
Set up the frontend testing infrastructure and write comprehensive tests for all components, hooks, and pages.

## Scope

### Test setup
- Install dev dependencies: `vitest`, `@testing-library/react`, `@testing-library/jest-dom`, `@testing-library/user-event`, `jsdom`, `msw` (Mock Service Worker for API mocking).
- Configure Vitest in `vite.config.ts` or separate `vitest.config.ts`:
  ```typescript
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
  }
  ```
- Create `src/test/setup.ts` with `@testing-library/jest-dom` import.
- Add test scripts to `package.json`: `"test": "vitest run"`, `"test:watch": "vitest"`.
- Set up MSW handlers for API mocking.

### Test categories

#### Auth tests
- `AuthContext.test.tsx`:
  - Provides default unauthenticated state.
  - `login()` sets user and token.
  - `logout()` clears user and token.
  - Restores session from localStorage on mount.
- `LoginPage.test.tsx`:
  - Renders username and password fields.
  - Submitting calls the login API.
  - Shows error message on failed login.
  - Redirects on successful login.
  - Disables submit button while loading.
- `ProtectedRoute.test.tsx`:
  - Redirects to /login when not authenticated.
  - Renders children when authenticated.

#### Layout tests
- `MainLayout.test.tsx`:
  - Renders top bar and outlet.
- `TopBar.test.tsx`:
  - Shows app title and username.
  - Shows admin navigation for admin users.
  - Shows user navigation for non-admin users.
  - Sign out button calls logout.

#### Products tests
- `ProductsPage.test.tsx`:
  - Renders product list from API data.
  - Shows loading state.
  - Shows error state.
  - Pagination controls work.
- `ProductDetailsPage.test.tsx`:
  - Renders product name and reviews.
  - Shows "Add Review" button when user hasn't reviewed.
  - Hides "Add Review" when user already reviewed.
  - Shows 404 for non-existent product.

#### Review tests
- `ReviewForm.test.tsx`:
  - Renders rating and text fields.
  - Validates required fields.
  - Validates rating range.
  - Validates text length.
  - Submits correctly in create mode.
  - Pre-fills in edit mode.
- `MyReviewsPage.test.tsx`:
  - Renders user's reviews.
  - Shows status badges.
  - Shows empty state.
  - Pagination works.

#### Admin tests
- `AdminReviewsPage.test.tsx`:
  - Renders reviews table.
  - Filter controls render and function.
  - Action buttons render based on status.
  - Status change triggers API call.
- `AdminUsersPage.test.tsx`:
  - Renders user table.
  - Ban/restore buttons render based on status.
  - Confirmation dialog on ban/unban.

#### Hook tests
- `useAuth.test.ts` — basic context functionality.
- `useProducts.test.ts` — fetch, loading, error states.
- `useReviews.test.ts` — CRUD operations and state management.
- `useAdminReviews.test.ts` — filter and pagination.
- `useUsers.test.ts` — ban/unban operations.

#### API client tests
- `apiClient.test.ts`:
  - Attaches auth token to requests.
  - Handles 401 response.
  - Correctly formats request bodies.
  - Correctly parses responses.

## Acceptance Criteria
- [ ] Test infrastructure is set up (Vitest, RTL, MSW).
- [ ] `npm test` runs all tests.
- [ ] All auth-related tests pass.
- [ ] All layout tests pass.
- [ ] All product page tests pass.
- [ ] All review tests pass.
- [ ] All admin page tests pass.
- [ ] All hook tests pass.
- [ ] API client tests pass.
- [ ] No test relies on a running backend (all API calls mocked).
- [ ] `npm run build` still succeeds.

## Notes / Edge Cases
- Use MSW (Mock Service Worker) for consistent API mocking — it intercepts `fetch` calls at the network level.
- Create mock data factories for consistent test data.
- Test error boundaries if implemented.
- Ensure tests are not coupled to implementation details (test behavior, not internals).
- Each test file should be colocated with the component or placed in a `__tests__` directory.
- Use `userEvent` from Testing Library for user interactions (more realistic than `fireEvent`).

## Dependencies
- Tasks 14–19 (all frontend features must be implemented).
- Can start with test infrastructure setup and write tests incrementally as features are completed.

## Testing Notes
- Run with `npm test` or `npx vitest run`.
- Watch mode: `npx vitest` for development.
- Coverage: `npx vitest --coverage` (add `@vitest/coverage-v8` if needed).
- Target: meaningful coverage for all user-facing functionality.
