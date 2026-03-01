# Task 24 — Products Page: Change to Vertical List Layout with Pagination

## Goal

Change the products page layout from a horizontal grid to a vertical list, retaining pagination support.

## Problem

- Products are currently displayed in a horizontal grid layout (`grid-template-columns: repeat(auto-fill, minmax(250px, 1fr))`).
- The layout should be a vertical, single-column list for a cleaner and more consistent browsing experience.

## Scope

### Frontend

#### `ProductsPage` component
- Update `src/pages/ProductsPage.tsx`:
  - Change the container CSS class from `products-grid` to `products-list` (or update the existing class styling).
  - Keep all existing functionality: loading state, error state, empty state, and pagination.

#### `ProductsPage.css`
- Update `src/pages/ProductsPage.css`:
  - Replace the grid layout (`.products-grid`) with a vertical flex/stack layout (`.products-list`).
  - Use `display: flex; flex-direction: column; gap: 1rem;` (or equivalent) to stack product cards vertically.
  - Remove the `grid-template-columns` rule.

#### `ProductCard` component
- Update `src/components/products/ProductCard.tsx` if needed to adapt to the vertical layout (e.g., adjust padding, width, or content alignment for a full-width card).
- No functional changes — the card should still show the product name and link to `/products/{id}`.

#### Pagination
- No changes to pagination behaviour. The `Pagination` component and `useProducts` hook remain the same.

### Testing

#### Frontend — Component tests
Update tests in `src/pages/ProductsPage.test.tsx`:
- Verify the products are rendered inside a vertical list container (`.products-list`).
- Existing tests (loading state, error state, empty state, pagination, product rendering) must continue to pass.

Update tests in `src/components/products/ProductCard.test.tsx` if the component markup changes:
- Verify the product card renders correctly in the new layout context.

## Acceptance Criteria

- [ ] Products are displayed in a single-column vertical list instead of a horizontal grid.
- [ ] Pagination continues to work correctly.
- [ ] Loading, error, and empty states are unaffected.
- [ ] Each product card spans the full width of the list container.
- [ ] All existing tests continue to pass.
- [ ] New or updated tests verify the vertical list layout.
- [ ] `npm run build` succeeds.

## Relevant Files

| Area | File |
|------|------|
| Frontend — Page | `frontend/src/pages/ProductsPage.tsx` |
| Frontend — Page styles | `frontend/src/pages/ProductsPage.css` |
| Frontend — ProductCard component | `frontend/src/components/products/ProductCard.tsx` |
| Frontend — Page tests | `frontend/src/pages/ProductsPage.test.tsx` |
| Frontend — ProductCard tests | `frontend/src/components/products/ProductCard.test.tsx` |
| Frontend — Hook | `frontend/src/hooks/useProducts.ts` |

## Dependencies

- Task 16 (Products list + product details page).

## Notes / Edge Cases

- This is a CSS/layout-only change — no backend changes are required.
- The vertical layout should be responsive and look good on both desktop and mobile viewports.
- Product card hover effects (box-shadow) should still work in the vertical layout.
- Ensure the product name link remains accessible and keyboard-navigable.
