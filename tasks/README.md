# Tasks — Execution Order & Dependencies

> **Project:** FTG12 Reviews Application  
> **Created:** 2026-03-01  
> **Total tasks:** 20 (estimated 1–3 hours each)

---

## Overview

The backlog is organized into 5 phases. Tasks within a phase may have dependencies on prior phases but are generally executable in the listed order. Some tasks within the same phase can be parallelized.

---

## Phase 1 — Backend Foundation (Tasks 01–04)

Establishes the project structure, database, mediator pattern, and authentication infrastructure.

| # | Task | Est. | Dependencies | File |
|---|------|------|-------------|------|
| 01 | Clean Architecture project restructuring | 2–3h | None | [task_01](task_01_clean_architecture_restructuring.md) |
| 02 | In-memory SQLite + EF Core + FluentMigrator (schema + seed) | 2–3h | 01 | [task_02](task_02_sqlite_efcore_fluentmigrator.md) |
| 03 | MediatR + validation pipeline + error handling middleware | 1–2h | 01 | [task_03](task_03_mediatr_validation_error_handling.md) |
| 04 | Password hashing (BCrypt) + JWT authentication config | 1–2h | 01, 02 | [task_04](task_04_password_hashing_jwt_auth.md) |

> **Note:** Tasks 02 and 03 can be done in parallel after Task 01 is complete.

---

## Phase 2 — Backend Features (Tasks 05–11)

Implements all API endpoints: auth, products, reviews, and admin user management.

| # | Task | Est. | Dependencies | File |
|---|------|------|-------------|------|
| 05 | Auth endpoints (login, logout, current user) | 2h | 01–04 | [task_05](task_05_auth_endpoints.md) |
| 06 | Authorization policies + banned user enforcement | 1–2h | 01–04 | [task_06](task_06_authorization_banned_users.md) |
| 07 | Products list endpoint | 1–2h | 01–04 | [task_07](task_07_products_list_endpoint.md) |
| 08 | Create + update review endpoints | 2–3h | 01–06 | [task_08](task_08_create_update_review.md) |
| 09 | List reviews (by product + my reviews) | 1–2h | 01–04, 07 | [task_09](task_09_list_reviews.md) |
| 10 | Admin reviews: filtered list + change status | 2h | 01–06 | [task_10](task_10_admin_reviews.md) |
| 11 | Admin user management (list, ban, unban) | 2h | 01–06 | [task_11](task_11_admin_user_management.md) |

> **Note:** Tasks 05, 06, 07 can be parallelized after Phase 1. Tasks 08–11 depend on 05 and 06 but are independent of each other.

---

## Phase 3 — Backend Tests (Tasks 12–13)

| # | Task | Est. | Dependencies | File |
|---|------|------|-------------|------|
| 12 | Backend unit tests (Application layer — xUnit) | 3h | 01–11 | [task_12](task_12_backend_unit_tests.md) |
| 13 | Backend integration tests (API endpoints — xUnit) | 3h | 01–11 | [task_13](task_13_backend_integration_tests.md) |

> **Note:** Tasks 12 and 13 can be done in parallel. They can also start incrementally as features from Phase 2 are completed.

---

## Phase 4 — Frontend Features (Tasks 14–19)

Implements the React frontend: auth, layout, products, reviews, and admin pages.

| # | Task | Est. | Dependencies | File |
|---|------|------|-------------|------|
| 14 | Frontend: routing + auth context + login page | 2–3h | Backend 05 | [task_14](task_14_frontend_routing_auth_login.md) |
| 15 | Frontend: main layout + remove healthcheck | 1–2h | 14 | [task_15](task_15_frontend_layout_remove_healthcheck.md) |
| 16 | Frontend: products list + product details | 2–3h | 14, 15, Backend 07, 09 | [task_16](task_16_frontend_products.md) |
| 17 | Frontend: add/edit review + my reviews page | 2–3h | 16, Backend 08, 09 | [task_17](task_17_frontend_reviews.md) |
| 18 | Frontend: admin reviews table + filters + actions | 2–3h | 15, Backend 10 | [task_18](task_18_frontend_admin_reviews.md) |
| 19 | Frontend: admin users management page | 1–2h | 15, Backend 11 | [task_19](task_19_frontend_admin_users.md) |

> **Note:** Tasks 18 and 19 can be done in parallel after Task 15. Task 17 depends on 16.

---

## Phase 5 — Frontend Tests (Task 20)

| # | Task | Est. | Dependencies | File |
|---|------|------|-------------|------|
| 20 | Frontend tests (Vitest + React Testing Library) | 3h | 14–19 | [task_20](task_20_frontend_tests.md) |

> **Note:** Test infrastructure setup can start early. Tests can be written incrementally as features are completed.

---

## Dependency Graph

```
Task 01 ──┬── Task 02 ──┐
          │             ├── Task 04 ──┬── Task 05 ──┬── Task 08
          │             │             │             ├── Task 09
          └── Task 03 ──┘             ├── Task 06 ──┼── Task 10
                                      │             ├── Task 11
                                      └── Task 07 ──┘
                                      
Task 05..11 ──┬── Task 12
              └── Task 13

Task 05 ── Task 14 ── Task 15 ──┬── Task 16 ── Task 17
                                ├── Task 18
                                └── Task 19

Task 14..19 ── Task 20
```

---

## Reference Documents

| Document | Description |
|----------|-------------|
| [architecture_improvements.md](architecture_improvements.md) | All architectural decisions and patterns |
| [initial_task.md](initial_task.md) | Original project setup plan |

---

## Seed Data Summary

| Table | Records |
|-------|---------|
| Users | Admin (admin, pwd=Admin), User1 (pwd=User1), User2 (pwd=User2), User3 (pwd=User3) |
| Products | Samsung RF28R7351SR Refrigerator, LG WM4500HBA Washing Machine, Panasonic NN-SN68KS Microwave, Breville BKE820XL Kettle |
| ReviewStatuses | Pending moderation (1), Approved (2), Rejected (3) |
| Reviews | 5–8 sample reviews across products and users |

---

## Tech Stack Summary

| Layer | Technology |
|-------|-----------|
| Backend framework | ASP.NET Core 10 (.NET 10, C# 14) |
| Architecture | Clean Architecture (Domain / Application / Infrastructure / Web) |
| Mediator | MediatR |
| Validation | FluentValidation |
| Database | In-memory SQLite (Microsoft.Data.Sqlite) |
| ORM | Entity Framework Core (SQLite provider) |
| Migrations | FluentMigrator |
| Auth | JWT Bearer tokens, BCrypt password hashing |
| Frontend framework | React 19, TypeScript, Vite |
| Frontend routing | React Router |
| Backend testing | xUnit, NSubstitute, FluentAssertions, WebApplicationFactory |
| Frontend testing | Vitest, React Testing Library, MSW |
