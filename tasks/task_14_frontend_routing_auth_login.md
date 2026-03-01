# Task 14 — Frontend: Routing + Auth Context + Login Page

## Goal
Set up React Router, implement an authentication context for managing user sessions, and build the login page.

## Scope

### Install dependencies
- `react-router-dom` — routing.
- No additional state management library needed.

### TypeScript types
- Create `src/types/auth.ts`:
  ```typescript
  export interface LoginRequest {
    username: string;
    password: string;
  }

  export interface LoginResponse {
    token: string;
    user: UserInfo;
  }

  export interface UserInfo {
    id: number;
    username: string;
    isAdministrator: boolean;
    isBanned: boolean;
  }
  ```

### Auth context
- Create `src/contexts/AuthContext.tsx`:
  - Provides: `user`, `token`, `isAuthenticated`, `isAdmin`, `login(username, password)`, `logout()`.
  - On mount: check `localStorage` for existing token, validate it (decode and check expiry), restore session.
  - `login()`: calls `POST /api/auth/login`, stores token in `localStorage`, sets user state.
  - `logout()`: calls `POST /api/auth/logout`, removes token from `localStorage`, redirects to `/login`.
- Create `src/hooks/useAuth.ts` — convenience hook wrapping `useContext(AuthContext)`.

### API client update
- Update `src/services/apiClient.ts`:
  - Set base URL from environment variable or default `http://localhost:7100`.
  - Add authorization header with JWT token from `localStorage`.
  - Add interceptor: on 401 response, clear token and redirect to `/login`.
  - Typed methods for auth endpoints: `login()`, `logout()`, `getCurrentUser()`.

### Protected route component
- Create `src/components/layout/ProtectedRoute.tsx`:
  - Wraps child routes.
  - If not authenticated, redirect to `/login`.
  - If authenticated, render children.
  - Optional: role-based protection (admin-only routes).

### Login page
- Create `src/pages/LoginPage.tsx`:
  - Form with username and password fields.
  - Submit button.
  - Error message display for invalid credentials.
  - Loading state during login request.
  - On successful login, redirect to `/` (which then redirects based on role).
  - If already authenticated, redirect away from login page.

### Router setup
- Update `src/App.tsx`:
  - Wrap in `BrowserRouter` and `AuthProvider`.
  - Define routes:
    - `/login` → `LoginPage` (public)
    - All other routes wrapped in `ProtectedRoute`
    - `/` → redirect to `/products` (or `/admin/reviews` for admin)
  - Placeholder pages for routes not yet implemented.

## Acceptance Criteria
- [ ] `npm run build` succeeds with no TypeScript errors.
- [ ] `/login` page renders with username and password fields.
- [ ] Submitting valid credentials stores the JWT token and redirects to the main app.
- [ ] Submitting invalid credentials shows an error message without page reload.
- [ ] Navigating to any protected route without authentication redirects to `/login`.
- [ ] Refreshing the browser restores the session from `localStorage`.
- [ ] Clicking "Sign out" clears the token and redirects to `/login`.
- [ ] The API client attaches the JWT token to all requests.
- [ ] A 401 response from any API call triggers automatic logout.

## Notes / Edge Cases
- Token validation on page load should check the expiry claim (`exp`). If expired, clear it and redirect to login.
- Do not decode the JWT on the server — just check `exp` client-side for session restoration. The actual validation happens server-side on every API call.
- The login form should disable the submit button while the request is in flight to prevent double submission.
- Use controlled form inputs.
- Handle network errors gracefully (show a generic error message if the backend is unreachable).
- Remove the existing healthcheck-related code from `App.tsx`. (Or defer to Task 15.)

## Dependencies
- Tasks 01–05 (backend auth endpoints must be functional).
- Frontend is independent of backend Tasks 06–11 for this task.

## Testing Notes
- Component test: LoginPage renders form fields and submit button.
- Component test: Submitting form with mock API calls the login function.
- Component test: Error message displays on failed login.
- Component test: ProtectedRoute redirects unauthenticated users.
- Hook test: useAuth provides correct state after login/logout.
- These tests will be written in Task 20.
