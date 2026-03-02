# AI Usage Report for the FTG12 Reviews Application

**Project:** FTG12 Reviews -- Full-Stack Product Review Platform  
**Date:** 2026-03-02  
**Stack:** ASP.NET Core 10 (C# 14) + React 19 (TypeScript), SQLite, MediatR, FluentValidation, JWT  

---

## Table of Contents

1. [Project Overview and Development Approach](#1-project-overview-and-development-approach)
2. [Tools and Models](#2-tools-and-models)
3. [Context Provided to the AI](#3-context-provided-to-the-ai)
4. [Key Prompts and Implementation Steps](#4-key-prompts-and-implementation-steps)
5. [MCP Servers and External Tools](#5-mcp-servers-and-external-tools)
6. [Observations and Conclusions](#6-observations-and-conclusions)

---

## 1. Project Overview and Development Approach

FTG12 Reviews is an educational full-stack application consisting of an ASP.NET Core 10 backend and a React 19 frontend with TypeScript. The application implements a consumer electronics review platform: users browse a product catalog, leave star ratings and text reviews, while administrators moderate content and manage user accounts.

The project was divided into 26 tasks organized into 5 phases: from building the foundational architecture to implementing the frontend and writing tests. Each task was documented as a separate Markdown file with a clear description of the goal, scope, acceptance criteria, and testing notes.

Development was carried out iteratively: first, the AI helped design the architecture and create the task plan, then tasks were executed sequentially using the AI assistant within VS Code.

---

## 2. Tools and Models

| Tool | Purpose |
|------|---------|
| GitHub Copilot (VS Code) | Primary AI assistant for code generation, refactoring, test writing, debugging |
| GitHub Copilot Agent Mode | Agent mode for multi-step tasks: repository analysis, feature implementation, test execution |
| Claude (via Copilot) | Language model used for code generation and architectural decisions |
| VS Code Terminal | Execution of dotnet and npm commands via the built-in terminal |
| Git + GitHub CLI (gh) | Version control, branch creation, and pull requests |

All interaction with the AI was conducted through VS Code with the GitHub Copilot extension. The core workflow was: write or select a prompt file, launch via Copilot Agent Mode, review the result, and adjust as needed.

---

## 3. Context Provided to the AI

Throughout the development process, the following context was provided to the AI assistant:

### Instruction Files (.github/instructions/)

- **copilot-sdk-csharp.instructions.md** -- guidance on working with the GitHub Copilot SDK for C#.
- **csharp.instructions.md** -- C# development conventions: naming (PascalCase, camelCase), file-scoped namespaces, nullable reference types, XML documentation, EF Core patterns, JWT authentication, FluentValidation, xUnit testing.
- **reactjs.instructions.md** -- React development standards: functional components, hooks, TypeScript, routing, form handling, testing.

### Prompt Files (.github/prompts/)

Reusable prompt files were created for standard operations:

- **create-task.prompt.md** -- create a new task file from a template.
- **implement-task.prompt.md** -- implement a task by number with automatic repository analysis, planning, implementation, and testing.
- **run-tests-and-app-check.prompt.md** -- verify the build, run tests, validate that the application works.
- **run-fullstack.prompt.md** -- start backend and frontend simultaneously.
- **push-changes-to-current-branch.prompt.md** -- commit and push changes.
- **create-pull-request.prompt.md** -- create a branch and a pull request.

### Project Documentation

- **project_description.md** -- full description of functionality, roles, and constraints.
- **backend-guide.md** -- backend structure, API endpoints, configuration.
- **frontend-guide.md** -- frontend structure, routes, components.
- **architecture_improvements.md** -- architectural decisions and Clean Architecture diagram.

### Task Files (tasks/)

26 task files, each containing: goal, scope (backend/frontend), acceptance criteria, dependencies, and testing notes. This gave the AI comprehensive context for every step.

---

## 4. Key Prompts and Implementation Steps

### Phase 0. Planning and Initial Setup

**Prompt:**  
> "Create an implementation plan for an educational project using .NET 10 + React TypeScript. Backend -- ASP.NET Core Web API with a /health endpoint. Frontend -- React 19 + TypeScript, Vite. Build and run documentation."

**Result:** Generated the initial_task.md file with a complete directory structure, a step-by-step implementation plan (15 steps), configuration files, and verification instructions. Created .gitignore, set up the solution file, project scaffolding.

**Accepted/Changed:** The plan was accepted in full. The only clarification was the use of the .slnx format instead of the classic .sln for the XML-based solution. Specific port assignments were added (7100 for API, 7200 for UI).

---

### Phase 1. Backend Foundation (Tasks 01--04)

#### Task 01 -- Clean Architecture

**Prompt:**  
> "/implement-task 01" (via the implement-task.prompt.md prompt file)

The prompt file instructed the AI to: study the repository, understand the architecture, find the task by number, plan the changes, implement, and test.

**Result:** Created the four-project structure: Domain (entities, repository interfaces), Application (CQRS handlers, validators, DTOs), Infrastructure (EF Core, repositories, services), Web/API (controllers, middleware). Set up project references and DependencyInjection.cs files for service registration.

**Accepted/Changed:** The structure was accepted in full. The AI correctly followed Clean Architecture dependency rules: Domain depends on nothing, Application depends only on Domain, Infrastructure depends on Application and Domain, Web depends on Application and Infrastructure.

#### Task 02 -- SQLite + EF Core + FluentMigrator

**Prompt:**  
> "/implement-task 02"

**Result:** Added NuGet packages. Created domain entities (User, Product, Review, ReviewStatus, BannedUser). Implemented AppDbContext with configurations. Created FluentMigrator migrations for the schema and seed data (4 users, 4 products, 8 reviews). Set up in-memory SQLite with a singleton connection.

**Accepted/Changed:** Result accepted. For the seed data, the AI pre-computed BCrypt password hashes and hardcoded them in the migrations, since the DI container is not available at migration time. This decision was documented in the task and accepted.

#### Task 03 -- MediatR + Validation + Error Handling

**Prompt:**  
> "/implement-task 03"

**Result:** Configured MediatR with assembly scanning. Created ValidationBehavior for automatic validation via FluentValidation. Implemented custom exceptions (ValidationException, NotFoundException, ForbiddenException, ConflictException). Created ExceptionHandlingMiddleware that maps exceptions to RFC 9457 Problem Details.

**Accepted/Changed:** Accepted without changes. Exception mapping: ValidationException -> 400, NotFoundException -> 404, ForbiddenException -> 403, ConflictException -> 409, UnauthorizedAccessException -> 401, all others -> 500 (no stack trace in the response).

#### Task 04 -- BCrypt + JWT

**Prompt:**  
> "/implement-task 04"

**Result:** Implemented IPasswordHasher/PasswordHasher (BCrypt), IJwtTokenService/JwtTokenService (token generation with claims: NameIdentifier, Name, Role), ICurrentUserService/CurrentUserService (reading identity from HttpContext). Configured JWT authentication in Program.cs.

**Accepted/Changed:** Accepted. The JWT secret is stored in appsettings.json (acceptable for an educational project). Token lifetime is set to 24 hours.

---

### Phase 2. Backend Features (Tasks 05--11)

#### Task 05 -- Auth Endpoints

**Prompt:**  
> "/implement-task 05"

**Result:** Implemented three endpoints: POST /api/auth/login (JWT issuance), POST /api/auth/logout (stateless, no-op on the server), GET /api/auth/me (current user with ban status from the database). Created MediatR commands LoginCommand and CurrentUserQuery with handlers and FluentValidation validators.

**Accepted/Changed:** Accepted. Login does not reveal whether the username or password was wrong -- it returns a generic "Invalid username or password" message. Username comparison is case-insensitive.

#### Task 06 -- Authorization + Banned Users

**Prompt:**  
> "/implement-task 06"

**Result:** Configured authorization policies. Created BannedUserBehavior -- a MediatR pipeline behavior that checks the user's ban status before executing commands marked with the IBannedUserCheck interface. The check queries the database rather than relying on token claims.

**Accepted/Changed:** The pipeline behavior approach was chosen over checking in each handler. This provides a single point of enforcement and adheres to the DRY principle.

#### Task 07 -- Products List

**Prompt:**  
> "/implement-task 07"

**Result:** Implemented GET /api/products (pagination) and GET /api/products/{id}. Created PaginatedList<T> -- a generic wrapper for paginated responses, reused by other endpoints.

**Accepted/Changed:** Accepted. Products are sorted by name. PaginatedList includes items, page, pageSize, totalCount, totalPages.

#### Tasks 08--11 -- Reviews, Admin Reviews, Admin Users

**Prompt (for each task):**  
> "/implement-task 08", "/implement-task 09", "/implement-task 10", "/implement-task 11"

**Result:** Implemented all CRUD operations for reviews, moderation endpoints (approve/reject), and user management (ban/unban). Each task followed the same pattern: MediatR command/query, handler, validator, DTO, controller, repository.

**Accepted/Changed:** The main adjustment was that when creating a duplicate review (same user + product), the DbUpdateException is caught and converted to a ConflictException (409). When updating a review, the status is reset to "Pending moderation".

---

### Phase 3. Backend Testing (Tasks 12--13)

#### Task 12 -- Unit Tests

**Prompt:**  
> "/implement-task 12"

**Result:** Created the FTG12_ReviewsApi.Application.Tests project (xUnit). Wrote tests for all handlers, validators, and behaviors. Used NSubstitute for mocking and FluentAssertions for assertions. Coverage includes: happy path, edge cases, validation errors, authorization, and exception handling.

**Accepted/Changed:** Accepted. Test organization mirrors the Application project structure. Test names describe behavior: Handle_WithValidCredentials_ReturnsToken, Handle_WhenUserNotFound_ThrowsNotFoundException.

#### Task 13 -- Integration Tests

**Prompt:**  
> "/implement-task 13"

**Result:** Created the FTG12_ReviewsApi.Api.Tests project with WebApplicationFactory. Wrote end-to-end tests for all API endpoints. A helper method CreateAuthenticatedClient generates a JWT and attaches it to the HttpClient. Each test class gets a clean in-memory database.

**Accepted/Changed:** Accepted. Tests validate the full pipeline: request deserialization -> handler -> database -> response serialization. Response codes, Problem Details format, and Content-Type headers are verified.

---

### Phase 4. Frontend (Tasks 14--19)

#### Task 14 -- Routing + Auth + Login Page

**Prompt:**  
> "/implement-task 14"

**Result:** Set up React Router. Created AuthContext (storing JWT in localStorage, automatic session restoration). Built LoginPage with a form, error handling, and loading state. Implemented ProtectedRoute for protected routes. The API client automatically attaches the token and handles 401 responses.

**Accepted/Changed:** Accepted. On a 401 response from any API call, automatic logout is triggered. Post-login redirect: regular users -> /products, administrators -> /admin/reviews.

#### Tasks 15--19 -- Layout, Products, Reviews, Admin Pages

**Prompt (for each task):**  
> "/implement-task 15" ... "/implement-task 19"

**Result:** Implemented: MainLayout with TopBar (navigation depends on role), ProductsPage with pagination, ProductDetailsPage with reviews and a form, MyReviewsPage, AdminReviewsPage with filters, AdminUsersPage with ban/unban.

**Accepted/Changed:** Component architecture is properly separated: pages (pages/) contain business logic, components (components/) are reusable. Hooks (hooks/) encapsulate API call logic and state management.

---

### Phase 5. Frontend Tests (Task 20)

**Prompt:**  
> "/implement-task 20"

**Result:** Set up the testing infrastructure: Vitest + React Testing Library + MSW (Mock Service Worker). Created 24 test files with 110 tests. MSW intercepts API requests and returns predictable responses.

**Accepted/Changed:** Accepted. MSW provides stable API mocking without a real backend. Test factories (factories.ts) generate test data.

---

### Additional Tasks (Tasks 21--26)

These tasks represented refinements and improvements identified during testing and review.

#### Task 21 -- Reject Approved Reviews

**Prompt:**  
> "/implement-task 21"

**Result:** Added the Approved -> Rejected transition. A "Reject" button is displayed for approved reviews. Updated backend and frontend tests.

#### Task 22 -- Fix Product Reviews Page + Edit

**Prompt:**  
> "/implement-task 22"

**Result:** Fixed a bug that occurred when loading the product page after creating a review. Added the ability to edit a review directly on the product page. The API response includes userReview -- the current user's review.

**Accepted/Changed:** This was a bug fix. The AI correctly diagnosed the problem: missing null checks and stale state after review creation. The fix included handling 409 Conflict on the frontend side.

#### Task 23 -- Own Reviews Visibility

**Prompt:**  
> "/implement-task 23"

**Result:** The current user's reviews are now displayed on the product page regardless of moderation status. Other users' reviews are shown only if approved. Added color-coded status badges (Pending -- yellow, Approved -- green, Rejected -- red).

#### Task 24 -- Products Vertical Layout

**Prompt:**  
> "/implement-task 24"

**Result:** Changed the products page layout from a horizontal grid to a vertical list. CSS-only changes with no functional modifications.

#### Task 25 -- Expand Seed Data

**Prompt:**  
> "/implement-task 25"

**Result:** Added migration M003_ExpandSeedData: 25 new products (29 total), reviews from every user for every product (87 reviews total). This ensured proper pagination testing (3 pages of 10 items). Updated tests that depend on data counts.

#### Task 26 -- Admin Restrictions

**Prompt:**  
> "/implement-task 26"

**Result:** Administrators can no longer ban themselves or other administrators. Administrators cannot create or edit reviews. The corresponding buttons are hidden on the frontend. Backend checks with ForbiddenException were added.

---

## 5. MCP Servers and External Tools

### MCP Servers

During development, the **Pylance** MCP server was available, providing static analysis for Python code. However, since this project does not use Python, actual interaction with this server was minimal.

The core MCP functionality was provided by built-in GitHub Copilot tools in VS Code:

- **Terminal commands** -- running dotnet build, dotnet test, npm install, npm test, npm run dev via the built-in terminal.
- **File operations** -- reading, creating, and editing project files.
- **Codebase search** -- grep search, semantic search, glob-pattern search.
- **Diagnostics** -- retrieving compilation and linting errors.

### Prompt Files as Automation Tools

The key external tool was the prompt files (`.github/prompts/`), which served as templates for repetitive operations:

| Prompt File | Purpose |
|-------------|---------|
| implement-task.prompt.md | Full task implementation cycle: analysis -> plan -> code -> tests -> verification |
| run-tests-and-app-check.prompt.md | Build validation, test execution, and application health check |
| run-fullstack.prompt.md | Start backend and frontend simultaneously |
| create-task.prompt.md | Generate a new task file from a template |
| create-pull-request.prompt.md | Create a branch and PR via Git/GitHub CLI |
| push-changes-to-current-branch.prompt.md | Commit and push current changes |

These files significantly accelerated development by eliminating the need to formulate a detailed prompt each time for standard operations.

---

## 6. Observations and Conclusions

### Which Prompts Worked Well

1. **A well-written prompt is the key to success.** This is perhaps the main lesson of the entire project. The quality of a prompt directly determines the quality of the result. Detailed task files with clear acceptance criteria, expected HTTP codes, request/response formats, and a list of test scenarios produced results that were as close to the expected outcome as possible. The more precise the specification, the fewer iterations of rework.

2. **The implement-task.prompt.md prompt file** turned out to be the most effective tool. It enforced a strict process: first study the repository and architecture, then plan the changes, then implement, then write tests, then run everything and verify. This eliminated the typical AI problem of jumping straight into coding without understanding the context.

3. **Instruction files (.github/instructions/).** Coding conventions (naming, formatting, patterns) were defined once and automatically applied to all subsequent tasks. The AI did not generate code using var instead of explicit types, did not create classes without file-scoped namespaces, and correctly applied nullable reference types.

4. **Incremental development and breaking work into small tasks.** The approach of "architecture first, then infrastructure, then features, then tests" produced better results than trying to implement everything at once. Overly complex tasks are better broken down into smaller ones. Small tasks are easier to work with, simpler to verify, and the AI makes fewer mistakes. Each task built on the results of the previous one, and the AI could use already-written code as a template for new components.

### Which Prompts Worked Poorly

1. **Poorly described prompts significantly complicate work.** Imprecise or ambiguous wording leads to unpredictable results. For example, a prompt for pushing changes to a branch might only push the files that the AI itself modified, rather than all accumulated changes, if this is not explicitly specified. A prompt for tests might create "green" tests that do not actually verify anything, if you do not ask the AI to run them and confirm correctness. Every inaccuracy in a prompt is a potential problem in the result.

2. **Cross-domain changes without decomposition.** When a task simultaneously affected both backend and frontend (for example, Task 22), the AI sometimes generated frontend code that was not fully consistent with the backend response. The solution is to execute the backend part and the frontend part sequentially, verifying compatibility between steps.

3. **Bug fixes without reproduction steps.** If a bug was not described concretely (with reproduction steps and expected behavior), the AI might propose a "fix" that did not address the root cause. In the case of Task 22, it was necessary to explicitly describe: "after creating a review, reopening the product page results in an error" with an indication of exactly which API response causes the problem.

4. **Refactoring existing code.** Refactoring prompts without a clear completion criterion led to excessive changes. The AI would start renaming variables or moving files that did not require modification.

### Prompting Patterns That Produced the Best Results

1. **"Architecture first, then implementation file by file."** The best results came from the following order: (a) describe the target architecture and dependencies, (b) implement each layer separately starting with Domain, (c) add tests for each layer. This approach minimizes the amount of rework.

2. **Mandatory result verification.** The AI's output should always be verified -- and it is best to ask the AI itself to do this as well. AI makes mistakes just like a human: it may miss an edge case, generate a non-working test, or forget to update a dependent component. The run-tests-and-app-check.prompt.md prompt, run after each task, allowed regressions to be caught quickly. This is significantly cheaper than debugging accumulated problems at the end.

3. **Declarative tasks with acceptance criteria.** The format "here is what should exist, here are the acceptance criteria, here is what needs to be tested" produced more predictable results than imperative instructions like "do this, then do that."

4. **Reusable prompt files.** Creating the implement-task.prompt.md template, which standardizes the workflow (study -> plan -> code -> tests -> verify), significantly improved the quality and consistency of results.

5. **Context via instruction files.** Placing conventions in .github/instructions/ with file type mapping (applyTo: "**/*.cs") ensures automatic rule application without the need to repeat them in every prompt.

6. **Choosing the right model for the task.** For each task, it is worth using the appropriate agent and the appropriate model. Heavy tasks -- complex architecture, cross-domain changes, non-trivial business logic -- are best handled by a more powerful (and expensive) model. Simple tasks -- boilerplate code generation, CSS tweaks, adding repetitive tests -- can be handled perfectly well by a standard model. The simpler the task, the simpler the model. This saves both time and resources.

7. **Explicit specification of patterns and libraries.** Stating in the task file: "use MediatR for CQRS, FluentValidation for validation, NSubstitute for mocking" eliminated ambiguity and prevented situations where the AI chose alternative libraries or approaches.

### General Conclusions

- AI is an excellent tool for accelerating development. With well-tuned prompts, significant time can be saved, especially when generating boilerplate code, CRUD operations, DTOs, validators, and tests. Instead of manually writing dozens of similar files, it is enough to correctly describe the task, and the AI delivers a working result in minutes.
- Beyond direct code generation, AI is a valuable learning tool. It can clearly show a junior developer how to solve a problem, how a technology works, and which patterns to apply. The ability to see the full step-by-step process (from specification to working code with tests) provides understanding that is difficult to obtain from documentation alone.
- The quality of the result is directly proportional to the quality of the prompt. A well-written prompt is not a formality but a key investment that determines success. A poorly described prompt leads to incomplete results, errors, and time lost on rework.
- Results must be verified. AI makes mistakes just like a human, and if a prompt is written imprecisely, the output can be unreliable. It is best to embed verification directly into the workflow -- asking the AI itself to run tests and validate functionality.
- Prompt files and instruction files are a capital investment of time that pays for itself many times over when working on a project with dozens of tasks.
- Breaking a project into small, well-specified tasks is a key success factor. Tasks estimated at 1-3 hours produce more predictable results and are easier to verify than large day-long tasks. Complex tasks are easier to work with when they are broken down into clear steps.
- Agent Mode with access to the terminal and file system allows the AI to independently detect and fix compilation errors, shortening the rework cycle.
- Tests not only verify code but also serve as a specification for the AI: the presence of existing tests helps the AI understand expected behavior and reproduce similar patterns in new tests.

---

*Document prepared as part of the FTG12 Reviews educational project.*
