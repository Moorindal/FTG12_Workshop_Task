# Task 15 — Frontend: Main Layout + Remove Healthcheck

## Goal
Create the main application layout with a top bar and remove the existing healthcheck UI.

## Scope

### Remove healthcheck
- Delete or replace `src/hooks/useHealthCheck.ts`.
- Delete `src/types/health.ts`.
- Remove healthcheck rendering from `App.tsx`.
- Clean up `App.css` — remove healthcheck-specific styles.
- Keep `src/services/apiClient.ts` (it will be updated for real API calls).

### Main layout
- Create `src/components/layout/MainLayout.tsx`:
  - Rendered as a wrapper for all authenticated routes (via `<Outlet />`).
  - Contains the top bar and a content area.

### Top bar
- Create `src/components/layout/TopBar.tsx`:
  - Left: Application title "FTG12 Reviews".
  - Right: Current username display + "Sign out" button.
  - Sign out calls `logout()` from the auth context.
  - If user is admin, show navigation links for admin pages (Reviews, Users).
  - If user is normal, show navigation links (Products, My Reviews).

### Navigation
- Links in the top bar:
  - **Admin**: "Reviews" → `/admin/reviews`, "Users" → `/admin/users`.
  - **User**: "Products" → `/products`, "My Reviews" → `/my-reviews`.
  - Both roles can access product pages.

### Styling
- Basic responsive layout using CSS (no external UI libraries required, but can use one if desired).
- Top bar: fixed at top, full width, contrasting background.
- Content area: centered, max width, padding.
- Use CSS modules or plain CSS files scoped to components.

### Route structure update
- Update `App.tsx` routes:
  ```
  /login                → LoginPage (public)
  /                     → MainLayout (protected)
    /products           → ProductsPage (placeholder)
    /products/:id       → ProductDetailsPage (placeholder)
    /my-reviews         → MyReviewsPage (placeholder)
    /admin/reviews      → AdminReviewsPage (placeholder, admin only)
    /admin/users        → AdminUsersPage (placeholder, admin only)
  ```
- Create placeholder pages (just a heading for now) for routes not yet implemented.

## Acceptance Criteria
- [ ] Healthcheck code is fully removed (hook, types, UI).
- [ ] `MainLayout` renders a top bar and content area.
- [ ] Top bar shows "FTG12 Reviews", current username, and "Sign out" button.
- [ ] Sign out works (clears session, redirects to login).
- [ ] Navigation links render based on user role (admin vs user).
- [ ] All placeholder pages are accessible via their routes.
- [ ] Admin routes are only accessible to admin users.
- [ ] `npm run build` succeeds.
- [ ] `npm run lint` passes.

## Notes / Edge Cases
- The existing `index.css` global styles can be kept and extended.
- Ensure the layout is responsive at common screen sizes (desktop and tablet).
- The "Sign out" button should be clearly visible and accessible.
- Navigation should highlight the currently active route.
- When a non-admin user navigates to `/admin/*`, redirect to `/products`.

## Dependencies
- Task 14 (routing, auth context, login page).

## Testing Notes
- Component test: MainLayout renders top bar with username.
- Component test: TopBar shows admin links for admin user.
- Component test: TopBar shows user links for non-admin user.
- Component test: Sign out button calls logout.
- These tests will be written in Task 20.
