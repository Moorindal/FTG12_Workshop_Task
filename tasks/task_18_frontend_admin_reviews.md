# Task 18 — Frontend: Admin Reviews Table with Filters + Actions

## Goal
Implement the admin reviews management page with a filterable table and per-row moderation actions.

## Scope

### API client methods
- Add to `apiClient.ts`:
  - `getAdminReviews(page, pageSize, statusId?, dateFrom?, dateTo?)` → `PaginatedResponse<Review>`.
  - `changeReviewStatus(reviewId, statusId)` → `Review`.

### Custom hook
- `src/hooks/useAdminReviews.ts`:
  - Fetches reviews with filters.
  - Exposes `{ reviews, loading, error, page, setPage, totalPages, filters, setFilters, refresh }`.

### Admin reviews page
- `src/pages/AdminReviewsPage.tsx`:
  - Accessible only to admin users (protected by admin route guard).
  - Filters panel:
    - Status dropdown: "All", "Pending moderation", "Approved", "Rejected".
    - Date range: "From" and "To" date inputs (HTML date pickers).
    - "Apply" button (or auto-apply on change).
    - "Clear filters" button.
  - Reviews table:
    - Columns: Product, User, Rating, Status, Text (truncated), Date, Actions.
    - Sortable by date (optional).
    - Paginated.

### Review status actions
- `src/components/admin/ReviewStatusActions.tsx`:
  - Renders action buttons based on current status:
    - If status is "Pending moderation": "Approve" button, "Reject" button.
    - If status is "Rejected": "Approve" button (re-approve).
    - If status is "Approved": no action (or show status chip only).
  - On action: call `changeReviewStatus()`, refresh the row.
  - Confirm dialog before status change (optional but recommended).
  - Loading state on action buttons.

### Admin reviews table component
- `src/components/admin/AdminReviewsTable.tsx`:
  - Renders the table with review data.
  - Each row uses `ReviewStatusActions`.
  - Responsive: collapses into card layout on small screens (optional).

## Acceptance Criteria
- [ ] Admin reviews page renders a filterable, paginated table.
- [ ] Status filter filters reviews by status.
- [ ] Date range filter filters reviews by creation date.
- [ ] Filters are combinable.
- [ ] "Approve" button changes review status to Approved.
- [ ] "Reject" button changes review status to Rejected (if actions needed).
- [ ] "Approve" button appears on Rejected reviews.
- [ ] Action success refreshes the row/table.
- [ ] Page is only accessible to admin users.
- [ ] Non-admin users are redirected away.
- [ ] `npm run build` succeeds.

## Notes / Edge Cases
- When approving/rejecting, the table should update without a full page reload. Either update the row in-place or refetch the current page.
- Clearing filters should reset the table to show all reviews.
- The date filter should handle timezone differences (dates sent as ISO strings).
- Truncate long review text in the table (show full text on hover or expand).
- Consider adding a visual indicator for the total count of pending reviews.
- Empty state: "No reviews match the selected filters."

## Dependencies
- Task 14 (routing, auth context).
- Task 15 (main layout with admin navigation).
- Backend Task 10 (admin reviews endpoints).

## Testing Notes
- Component test: AdminReviewsPage renders table with reviews.
- Component test: Filter controls update the query.
- Component test: Action buttons render based on status.
- Component test: Clicking "Approve" triggers API call.
- These tests will be written in Task 20.
