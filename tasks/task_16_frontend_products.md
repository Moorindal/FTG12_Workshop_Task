# Task 16 — Frontend: Products List + Product Details Page

## Goal
Implement the products list page with pagination and the product details page showing reviews.

## Scope

### TypeScript types
- Create `src/types/product.ts`:
  ```typescript
  export interface Product {
    id: number;
    name: string;
  }

  export interface PaginatedResponse<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
  }
  ```
- Create `src/types/review.ts`:
  ```typescript
  export interface Review {
    id: number;
    productId: number;
    productName: string;
    userId: number;
    username: string;
    statusId: number;
    statusName: string;
    rating: number;
    text: string;
    createdAt: string;
  }
  ```

### API client methods
- Add to `apiClient.ts`:
  - `getProducts(page, pageSize)` → `PaginatedResponse<Product>`.
  - `getProduct(id)` → `Product`.
  - `getProductReviews(productId, page, pageSize)` → `PaginatedResponse<Review>`.

### Custom hooks
- `src/hooks/useProducts.ts` — fetches paginated products, exposes `{ products, loading, error, page, setPage, totalPages }`.
- `src/hooks/useProductReviews.ts` — fetches reviews for a product, exposes `{ reviews, loading, error, page, setPage, totalPages }`.

### Products list page
- `src/pages/ProductsPage.tsx`:
  - Displays a list/grid of product cards.
  - Each card shows product name and links to product details.
  - Pagination controls (Previous / Next / page numbers).
  - Loading and error states.

### Product card component
- `src/components/products/ProductCard.tsx`:
  - Product name.
  - Link to `/products/{id}`.
  - Optional: average rating display.

### Product details page
- `src/pages/ProductDetailsPage.tsx`:
  - Shows product name.
  - Lists approved reviews with rating, username, text, date.
  - Pagination for reviews.
  - "Add Review" button (if user hasn't reviewed this product yet).
  - Loading and error states.
  - 404 handling for non-existent product.

### Review card component
- `src/components/reviews/ReviewCard.tsx`:
  - Rating (stars or number).
  - Username.
  - Review text.
  - Date.

### Pagination component
- Create `src/components/common/Pagination.tsx`:
  - Reusable pagination controls.
  - Props: `page`, `totalPages`, `onPageChange`.

## Acceptance Criteria
- [ ] `/products` page displays a paginated list of products.
- [ ] Clicking a product navigates to `/products/{id}`.
- [ ] Product details page shows product name and approved reviews.
- [ ] Pagination works on both products list and reviews list.
- [ ] Loading spinners/skeletons display while fetching data.
- [ ] Error messages display for failed API calls.
- [ ] 404 page/message for non-existent product.
- [ ] "Add Review" button shows/hides based on whether the user has already reviewed.
- [ ] `npm run build` succeeds.

## Notes / Edge Cases
- The "Add Review" button should check the current user's reviews for this product. This can be done via the API response (include a flag) or by fetching "my reviews" and checking locally.
- Empty product list: show a friendly "No products found" message.
- Empty reviews: show "No reviews yet. Be the first to review!"
- Review dates should be formatted in a human-readable way (e.g., "Mar 1, 2026").
- Star ratings can be displayed as filled/empty star icons or simple "4/5" text.

## Dependencies
- Task 14 (routing, auth context).
- Task 15 (main layout).
- Backend Tasks 07, 09 (products + reviews endpoints must work).

## Testing Notes
- Component test: ProductsPage renders product cards from mock data.
- Component test: Pagination controls update the page.
- Component test: ProductDetailsPage renders reviews.
- Component test: "Add Review" button visibility logic.
- Hook test: useProducts fetches data and handles errors.
- These tests will be written in Task 20.
