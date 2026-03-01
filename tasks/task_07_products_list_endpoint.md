# Task 07 — Products List Endpoint

## Goal
Implement the endpoint to list all products with pagination support.

## Scope

### Endpoint
- `GET /api/products` — returns paginated list of products.
- `GET /api/products/{id}` — returns a single product by ID.

### Query parameters (list)
- `page` (int, default 1, min 1)
- `pageSize` (int, default 10, min 1, max 50)

### Response (list)
```json
{
  "items": [
    { "id": 1, "name": "Samsung RF28R7351SR Refrigerator" },
    { "id": 2, "name": "LG WM4500HBA Washing Machine" }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 4,
  "totalPages": 1
}
```

### Response (single)
```json
{
  "id": 1,
  "name": "Samsung RF28R7351SR Refrigerator"
}
```

### MediatR requests
- `GetProductsQuery` → `GetProductsQueryHandler` (in Application/Products/Queries/).
- `GetProductByIdQuery` → `GetProductByIdQueryHandler` (in Application/Products/Queries/).

### DTOs
- `ProductDto` — `Id`, `Name`.
- `PaginatedList<T>` — generic paginated response wrapper in `Application/Common/Models/`.

### Controller
- Create `ProductsController` in the Web/API project.
- `[Authorize]` — requires authentication.

### Repository
- `IProductRepository.GetPagedAsync(page, pageSize)` → returns products + total count.
- `IProductRepository.GetByIdAsync(id)` → returns single product or null.

## Acceptance Criteria
- [ ] `GET /api/products` returns paginated product list (200).
- [ ] `GET /api/products?page=1&pageSize=2` returns first 2 products with correct pagination metadata.
- [ ] `GET /api/products/{id}` returns single product (200) or 404 if not found.
- [ ] Requires authentication (401 without token).
- [ ] `PaginatedList<T>` is reusable for other paginated endpoints.
- [ ] `dotnet build` succeeds.
- [ ] Seed data products are returned.

## Notes / Edge Cases
- `page` < 1 or `pageSize` < 1 should return validation error (400).
- `pageSize` > 50 should be clamped to 50 or return validation error.
- Empty result (no products matching) should return `items: []` with `totalCount: 0`.
- Products are ordered by `Name` alphabetically.

## Dependencies
- Task 01 (project structure).
- Task 02 (Product entity, database, seed data).
- Task 03 (MediatR, validation, error handling).
- Task 04 (JWT auth — endpoints require authentication).

## Testing Notes
- Unit test: `GetProductsQueryHandler` returns correct paginated results from mock repository.
- Unit test: `GetProductByIdQueryHandler` throws `NotFoundException` for missing product.
- Unit test: Pagination validator rejects invalid page/pageSize.
- Integration test: `GET /api/products` with auth token returns seed products.
- Integration test: `GET /api/products` without auth returns 401.
