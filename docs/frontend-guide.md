# Frontend — Build and Run Guide

This guide describes how to build and run the React 19 + TypeScript frontend.

---

## Prerequisites

| Requirement | Version | Verify |
|------------|---------|--------|
| Node.js | 20.19.0 or later (or 22.12.0+) | `node --version` |
| npm | 10.0 or later | `npm --version` |

Download Node.js from [https://nodejs.org/](https://nodejs.org/).

---

## Project Structure

All frontend files are in the `frontend/` folder at the repository root:

```
frontend/
├── index.html              # Vite HTML entry point
├── package.json            # Dependencies and scripts
├── vite.config.ts          # Vite configuration (port 7200)
├── tsconfig.json           # TypeScript root config
├── tsconfig.app.json       # App compilation settings
├── tsconfig.test.json      # Test compilation settings
├── tsconfig.node.json      # Vite/Node tooling settings
├── eslint.config.js        # ESLint rules
├── public/                 # Static assets
└── src/
    ├── main.tsx            # React DOM entry point
    ├── App.tsx             # Root component with routing
    ├── App.css             # Root component styles
    ├── index.css           # Global styles
    ├── components/
    │   ├── admin/          # Admin panel components
    │   │   ├── AdminReviewsTable.tsx
    │   │   ├── ReviewStatusActions.tsx
    │   │   ├── UserManagement.tsx
    │   │   └── UserRow.tsx
    │   ├── common/         # Shared UI components
    │   │   └── Pagination.tsx
    │   ├── layout/         # App shell and route guards
    │   │   ├── MainLayout.tsx
    │   │   ├── ProtectedRoute.tsx
    │   │   └── TopBar.tsx
    │   ├── products/
    │   │   └── ProductCard.tsx
    │   └── reviews/
    │       ├── ReviewCard.tsx
    │       └── ReviewForm.tsx
    ├── contexts/
    │   └── AuthContext.tsx      # Auth state provider
    ├── hooks/
    │   ├── useAdminReviews.ts   # Admin review management
    │   ├── useAuth.ts           # Auth context consumer
    │   ├── useProductReviews.ts # Product review fetching
    │   ├── useProducts.ts       # Product listing
    │   ├── useReviews.ts        # User's own reviews
    │   └── useUsers.ts          # Admin user management
    ├── pages/
    │   ├── AdminReviewsPage.tsx
    │   ├── AdminUsersPage.tsx
    │   ├── LoginPage.tsx
    │   ├── MyReviewsPage.tsx
    │   ├── ProductDetailsPage.tsx
    │   └── ProductsPage.tsx
    ├── services/
    │   └── apiClient.ts        # Backend API client
    ├── test/
    │   ├── factories.ts        # Test data factories
    │   ├── handlers.ts         # MSW mock API handlers
    │   ├── server.ts           # MSW server setup
    │   └── setup.ts            # Vitest setup (jest-dom, MSW)
    └── types/
        ├── auth.ts
        ├── product.ts
        ├── review.ts
        └── user.ts
```

### Key Libraries

| Library | Purpose |
|---------|---------|
| React 19 | UI library |
| React Router 7 | Client-side routing |
| Vite 7 | Build tool and dev server |
| TypeScript 5.9 | Type safety |
| Vitest 4 | Test framework |
| @testing-library/react | Component testing |
| MSW 2 | API mocking in tests |

---

## Install Dependencies

From the repository root:

```bash
cd frontend
npm install
```

---

## Run (Development)

```bash
cd frontend
npm run dev
```

The dev server starts on **`http://localhost:7200`** with hot module replacement (HMR).

**Expected console output:**

```
  VITE v7.x.x  ready in XXX ms

  ➜  Local:   http://localhost:7200/
  ➜  Network: use --host to expose
  ➜  press h + enter to show help
```

> **Note:** The backend must be running on `http://localhost:7100` for API calls to work. See [backend-guide.md](backend-guide.md) for instructions.

---

## Build (Production)

```bash
cd frontend
npm run build
```

Outputs optimized static files to the `dist/` folder.

---

## Run Tests

```bash
cd frontend
npm test
```

Runs all Vitest tests (24 test files, 110 tests). Tests use MSW to mock backend API responses.

For watch mode:

```bash
npm run test:watch
```

---

## Linting

```bash
cd frontend
npm run lint
```

Runs ESLint with TypeScript and React rules.

---

## Application Features

### Authentication

The app uses JWT-based authentication against the backend API.

**Login flow:**

1. User enters username and password on the login page (`/login`).
2. Frontend calls `POST /api/auth/login` with credentials.
3. On success, the JWT token is stored in `localStorage` and user info is saved in React context.
4. On page reload, `AuthContext` checks for a valid token and calls `GET /api/auth/me` to restore the session.
5. On 401 responses, the token is cleared and the user is redirected to `/login`.

**Post-login redirect:**

- Admin users → `/admin/reviews`
- Regular users → `/products`

**Logout:** Calls `POST /api/auth/logout`, clears `localStorage`, and resets auth state.

### Routes

| Path | Component | Access | Description |
|------|-----------|--------|-------------|
| `/login` | `LoginPage` | Public | Username/password login form |
| `/` | — | Authenticated | Redirects to `/products` |
| `/products` | `ProductsPage` | Authenticated | Paginated product grid |
| `/products/:id` | `ProductDetailsPage` | Authenticated | Product details with reviews and review form |
| `/my-reviews` | `MyReviewsPage` | Authenticated | Current user's reviews (all statuses) |
| `/admin/reviews` | `AdminReviewsPage` | Admin only | Review moderation with status/date filters |
| `/admin/users` | `AdminUsersPage` | Admin only | User management with ban/unban actions |

### Protected Routes

- **`ProtectedRoute`** — Requires authentication (valid token + user). Redirects to `/login` if not authenticated.
- **Admin routes** — Additionally checks `isAdmin`. Redirects to `/products` if user is not an admin.

### Navigation

The `TopBar` component displays role-appropriate navigation links:

- **All authenticated users:** Products, My Reviews, Sign Out
- **Admin users additionally:** Reviews (admin), Users (admin)

### Products

- Paginated product grid using `ProductCard` components.
- Product detail page showing product name and approved reviews.

### Reviews

- **Create:** Submit a review on a product detail page (one review per user per product).
- **Edit:** Update own reviews on the "My Reviews" page (re-submits for moderation).
- **Star rating:** 1–5 scale with text content.
- **Statuses:** Pending moderation, Approved, Rejected — displayed with color-coded badges.

### Admin — Review Moderation

- Filterable table: filter by status, date range.
- Approve or reject reviews via `ReviewStatusActions` component.
- Paginated results.

### Admin — User Management

- User list showing username, role, ban status, and creation date.
- Ban/unban toggle per user via `UserRow` component.

---

## API Client Configuration

The backend URL is resolved in `src/services/apiClient.ts` from the `VITE_API_BASE_URL` environment variable, falling back to `http://localhost:7100`:

```typescript
const API_BASE_URL =
  (import.meta as { env?: { VITE_API_BASE_URL?: string } }).env?.VITE_API_BASE_URL ??
  'http://localhost:7100';
```

To override the default, set `VITE_API_BASE_URL` in a `.env.local` file at the root of the `frontend/` folder:

```
VITE_API_BASE_URL=http://localhost:7100
```

### API Endpoints Used

| Method | Endpoint | Purpose |
|--------|----------|---------|
| `POST` | `/api/auth/login` | Login |
| `POST` | `/api/auth/logout` | Logout |
| `GET` | `/api/auth/me` | Get current user |
| `GET` | `/api/products?page=&pageSize=` | List products |
| `GET` | `/api/products/:id` | Get single product |
| `GET` | `/api/products/:id/reviews?page=&pageSize=` | Product reviews |
| `POST` | `/api/reviews` | Create review |
| `PUT` | `/api/reviews/:id` | Update review |
| `GET` | `/api/reviews/my?page=&pageSize=` | User's own reviews |
| `GET` | `/api/admin/reviews?page=&pageSize=&statusId=&dateFrom=&dateTo=` | Admin: list reviews |
| `PUT` | `/api/admin/reviews/:id/status` | Admin: change review status |
| `GET` | `/api/admin/users` | Admin: list users |
| `POST` | `/api/admin/users/:id/ban` | Admin: ban user |
| `POST` | `/api/admin/users/:id/unban` | Admin: unban user |

---

## Available Scripts

| Script | Command | Description |
|--------|---------|-------------|
| `npm run dev` | `vite` | Start dev server on port 7200 |
| `npm run build` | `tsc -b && vite build` | Type-check and build for production |
| `npm run lint` | `eslint .` | Run ESLint |
| `npm run preview` | `vite preview` | Preview production build |
| `npm test` | `vitest run` | Run all tests once |
| `npm run test:watch` | `vitest` | Run tests in watch mode |

---

## Stopping the Dev Server

Press `Ctrl+C` in the terminal where the dev server is running, or press `q + Enter`.
