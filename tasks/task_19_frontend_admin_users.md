# Task 19 — Frontend: Admin Users Management Page

## Goal
Implement the admin users management page with ban/unban functionality.

## Scope

### TypeScript types
- Create/update `src/types/user.ts`:
  ```typescript
  export interface User {
    id: number;
    username: string;
    isAdministrator: boolean;
    isBanned: boolean;
    bannedAt: string | null;
    createdAt: string;
  }
  ```

### API client methods
- Add to `apiClient.ts`:
  - `getUsers()` → `User[]`.
  - `banUser(userId)` → `User`.
  - `unbanUser(userId)` → `User`.

### Custom hook
- `src/hooks/useUsers.ts`:
  - Fetches all users.
  - Provides ban/unban methods.
  - Exposes `{ users, loading, error, banUser, unbanUser, refresh }`.

### Admin users page
- `src/pages/AdminUsersPage.tsx`:
  - Accessible only to admin users.
  - Displays a table/list of all users.
  - Columns: Username, Role (Admin/User), Status (Active/Banned), Banned At, Actions.
  - Admin users are visually distinguished (badge or icon).

### User row component
- `src/components/admin/UserRow.tsx`:
  - Renders user information and action buttons.
  - If user is not banned: "Ban" button (red/warning style).
  - If user is banned: "Restore" button (green/success style) + shows ban date.
  - Confirm dialog before banning/unbanning: "Are you sure you want to ban/restore {username}?"
  - Loading state during action.

### User management table
- `src/components/admin/UserManagement.tsx`:
  - Table layout with all users.
  - Status indicators:
    - Active: green badge.
    - Banned: red badge with ban date.
  - Admin role indicator: shield icon or "Admin" badge.

## Acceptance Criteria
- [ ] Admin users page renders a table of all users.
- [ ] Each user shows username, role, ban status, and ban date.
- [ ] "Ban" button is shown for non-banned users.
- [ ] "Restore" button is shown for banned users.
- [ ] Confirmation dialog appears before ban/unban action.
- [ ] Banning a user updates the table row without full reload.
- [ ] Unbanning a user updates the table row.
- [ ] Already-banned user shows 409 error handled gracefully.
- [ ] Page is only accessible to admin users.
- [ ] `npm run build` succeeds.

## Notes / Edge Cases
- Banning an admin user: technically allowed, but the UI should show a stronger warning: "This user is an administrator. Are you sure?"
- After banning, the user's existing JWT remains valid until expiry, but they will be blocked from creating/updating reviews by the backend.
- If the admin bans themselves, they should still be able to access admin pages (admins are only blocked from review creation, if at all). Clarify the business rule.
- Handle network errors gracefully (show error toast/message and allow retry).
- Show timestamps in human-readable format.

## Dependencies
- Task 14 (routing, auth context).
- Task 15 (main layout with admin navigation).
- Backend Task 11 (admin user management endpoints).

## Testing Notes
- Component test: AdminUsersPage renders user table.
- Component test: Ban button calls API and updates row.
- Component test: Restore button calls API and updates row.
- Component test: Confirmation dialog appears before action.
- These tests will be written in Task 20.
