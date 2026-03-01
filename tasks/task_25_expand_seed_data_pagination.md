# Task 25 — Expand Seed Data: Add Products and Reviews for Pagination Testing

## Goal

Expand the seed data migration to include 25 additional products and reviews from every user for every product. Ensure all paginated views across the project use a page size of 10 items. Update tests to account for the expanded data set.

## Problem

- The current seed data contains only 4 products and 8 reviews, which is insufficient to verify pagination behaviour.
- Not all users have reviews for all products, making it difficult to test the "My Reviews" pagination.
- Pagination page size defaults need to be consistently set to 10 across the entire project (backend and frontend).

## Current State

### Seed data (`M002_SeedData.cs`)
- **Users:** 4 (Admin, User1, User2, User3)
- **Products:** 4 (Samsung Refrigerator, LG Washing Machine, Panasonic Microwave, Breville Kettle)
- **Reviews:** 8 (partial coverage — not every user has a review for every product)

### Pagination defaults
- Backend queries (`GetProductsQuery`, `GetReviewsByProductQuery`, `GetMyReviewsQuery`, `GetAllReviewsQuery`): `PageSize = 10`
- Backend controllers (`ProductsController`, `ReviewsController`, `AdminReviewsController`): `pageSize = 10`
- Frontend hooks (`useProducts`, `useProductReviews`, `useMyReviews`, `useAdminReviews`): `pageSize = 10`

All defaults are already 10. Verify they remain consistent after changes.

## Scope

### Backend

#### New migration `M003_ExpandSeedData.cs`
Create a new FluentMigrator migration in `backend/src/FTG12_ReviewsApi.Infrastructure/Migrations/`:

- **Add 25 new products** (IDs 5–29). Use realistic appliance/electronics model names, for example:
  - Dyson V15 Detect Vacuum
  - Bosch Serie 6 Dishwasher
  - Philips 3200 Espresso Machine
  - iRobot Roomba j7+ Robot Vacuum
  - KitchenAid Artisan Stand Mixer
  - De'Longhi Magnifica Evo Coffee Machine
  - Sony WH-1000XM5 Headphones
  - Ninja Foodi Air Fryer
  - Instant Pot Duo Plus
  - Vitamix A3500 Blender
  - Bose SoundLink Revolve+ Speaker
  - Samsung QN90B Neo QLED TV
  - LG C3 OLED TV
  - Apple AirPods Pro 2
  - Garmin Venu 3 Smartwatch
  - Dyson Purifier Hot+Cool
  - Shark Navigator Lift-Away Vacuum
  - Cuisinart TOA-65 Air Fryer Toaster Oven
  - Nespresso Vertuo Next Coffee Machine
  - Weber Spirit II E-310 Gas Grill
  - Philips Sonicare DiamondClean Toothbrush
  - Ecovacs Deebot X2 Omni Robot Vacuum
  - Tineco Floor One S5 Wet Dry Vacuum
  - JBL Charge 5 Bluetooth Speaker
  - Anker Soundcore Liberty 4 Earbuds
  
- **Add reviews from every non-admin user (User1, User2, User3) for every product** (original 4 + new 25 = 29 products).
  - Each user should have one review per product.
  - Skip combinations that already exist in `M002_SeedData` (User1 already reviewed products 1, 2, 4; User2 already reviewed products 1, 3, 4; User3 already reviewed products 2, 3).
  - Use varied ratings (1–5) and realistic review text.
  - Use a mix of statuses: most should be `Approved` (StatusId=2), some `Pending moderation` (StatusId=1), and a few `Rejected` (StatusId=3) to ensure status filtering is testable.
  
- Expected totals after migration:
  - **Products:** 29 (4 existing + 25 new) — ensures 3 pages at pageSize=10.
  - **Reviews per user:** 29 (one per product) — ensures 3 pages of "My Reviews" at pageSize=10.
  - **Total reviews:** 87 (3 users × 29 products) — sufficient for pagination testing on admin reviews.

#### Verify pagination defaults
- Confirm all backend query defaults remain `PageSize = 10`:
  - `GetProductsQuery`
  - `GetReviewsByProductQuery`
  - `GetMyReviewsQuery`
  - `GetAllReviewsQuery`
- Confirm all backend controller defaults remain `pageSize = 10`:
  - `ProductsController.GetAllAsync`
  - `ReviewsController.GetMyReviewsAsync`
  - `ReviewsController.GetReviewsByProductAsync`
  - `AdminReviewsController.GetAllAsync`
- If any default is not 10, update it to 10.

### Frontend

#### Verify pagination defaults
- Confirm all frontend hook defaults remain `pageSize = 10`:
  - `useProducts`
  - `useProductReviews`
  - `useMyReviews`
  - `useAdminReviews`
- If any default is not 10, update it to 10.

### Testing

#### Backend — Integration tests
Update tests in `FTG12_ReviewsApi.Api.Tests` that depend on seed data counts:

- **Product endpoint tests** — update expected product counts to reflect 29 products.
- **Review endpoint tests** — update expected review counts and assertions that depend on specific seed data (e.g., `GetReviewsByProduct_ReturnsApprovedOnly` may return more results).
- **Admin review endpoint tests** — update expected counts for the admin reviews list.
- **My reviews endpoint tests** — update expected counts for user-specific review lists.
- Add a test verifying pagination works with >10 products (e.g., page 1 returns 10 items, page 3 returns remaining items).
- Add a test verifying "My Reviews" pagination with >10 reviews per user.

#### Backend — Unit tests
- Unit tests in `FTG12_ReviewsApi.Application.Tests` use mocked data and should not be affected by seed data changes. Verify they still pass.
- Unit tests in `FTG12_ReviewsApi.Tests` (if they depend on seed counts) should be updated.

#### Frontend tests
- Frontend tests use MSW mocks and should not be affected by seed data changes. Verify they still pass.

## Acceptance Criteria

- [ ] 25 new products are added via a new migration (`M003_ExpandSeedData`).
- [ ] Every non-admin user has a review for every product (29 reviews per user, 87 total).
- [ ] Reviews have varied ratings and a mix of statuses (mostly Approved, some Pending, a few Rejected).
- [ ] All pagination defaults are consistently set to 10 items per page across backend and frontend.
- [ ] Products page shows multiple pages of results (at least 3 pages).
- [ ] "My Reviews" page shows multiple pages of results.
- [ ] Admin Reviews page shows multiple pages of results.
- [ ] All existing tests continue to pass (updated where needed for new data counts).
- [ ] New integration tests verify pagination with the expanded data set.
- [ ] `dotnet build` succeeds.
- [ ] `npm run build` succeeds.

## Relevant Files

| Area | File |
|------|------|
| Backend — New migration | `backend/src/FTG12_ReviewsApi.Infrastructure/Migrations/M003_ExpandSeedData.cs` (new) |
| Backend — Existing seed data | `backend/src/FTG12_ReviewsApi.Infrastructure/Migrations/M002_SeedData.cs` |
| Backend — Product query | `backend/src/FTG12_ReviewsApi.Application/Products/Queries/GetProductsQuery.cs` |
| Backend — Review queries | `backend/src/FTG12_ReviewsApi.Application/Reviews/Queries/GetReviewsByProductQuery.cs` |
| Backend — Review queries | `backend/src/FTG12_ReviewsApi.Application/Reviews/Queries/GetMyReviewsQuery.cs` |
| Backend — Review queries | `backend/src/FTG12_ReviewsApi.Application/Reviews/Queries/GetAllReviewsQuery.cs` |
| Backend — Controllers | `backend/src/FTG12_ReviewsApi/Controllers/ProductsController.cs` |
| Backend — Controllers | `backend/src/FTG12_ReviewsApi/Controllers/ReviewsController.cs` |
| Backend — Controllers | `backend/src/FTG12_ReviewsApi/Controllers/AdminReviewsController.cs` |
| Backend — Integration tests | `backend/tests/FTG12_ReviewsApi.Api.Tests/Reviews/ReviewEndpointTests.cs` |
| Backend — Integration tests | `backend/tests/FTG12_ReviewsApi.Api.Tests/Admin/AdminReviewEndpointTests.cs` |
| Backend — Unit tests | `backend/src/FTG12_ReviewsApi.Tests/Products/GetProductsTests.cs` |
| Frontend — Hooks | `frontend/src/hooks/useProducts.ts` |
| Frontend — Hooks | `frontend/src/hooks/useProductReviews.ts` |
| Frontend — Hooks | `frontend/src/hooks/useReviews.ts` |
| Frontend — Hooks | `frontend/src/hooks/useAdminReviews.ts` |

## Dependencies

- Task 02 (SQLite, EF Core, FluentMigrator setup).
- Task 07 (Products list endpoint).
- Task 09 (List reviews endpoints).
- Task 16 (Frontend products page with pagination).

## Notes / Edge Cases

- Create a **new migration** (`M003`) rather than modifying `M002_SeedData` to preserve migration history and avoid re-running existing migrations.
- The `Down()` method of the new migration should delete only the rows added by `M003` (e.g., delete products with ID > 4 and their associated reviews, or delete reviews not in the original 8).
- Existing integration tests that assert exact counts (e.g., `result.Items.Should().HaveCount(2)`) will need updating to match the new totals.
- Be careful with review ID assumptions — the auto-increment IDs for new reviews will start after the existing 8 reviews.
- The `CreateReview_WithValidData_Returns201` integration test creates a new review; ensure it does not conflict with pre-seeded data (User3/Product 1 combination may now be occupied).
