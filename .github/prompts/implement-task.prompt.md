---
description: Implement task by number with full repo/architecture study + add/update backend & frontend tests + verify run + verify all tests (no commit)
---

You are working inside a VS Code workspace that is a Git repository containing backend (.NET) and frontend code.

The task number is provided as the first argument:
${1}

If no task number is provided, STOP and inform the user:
"Please provide a task number. Example: /implement-task 1234"

Global rules:
- Do not commit changes and do not create branches.
- Do not add unnecessary comments to the code.
- Do not refactor unrelated code or reformat files without need.
- Keep changes minimal, task-focused, and consistent with existing conventions.
- Prefer existing libraries, patterns, and test frameworks already used in the repository.
- If requirements are unclear, infer behavior from existing patterns and similar past tasks in the repo.

Goal:
- Study the current repository architecture and previously implemented tasks/features.
- Implement task ${1} according to repository conventions and requirements discovered in-code/docs.
- Verify the implemented functionality runs and works end-to-end where applicable.
- Add/extend/fix automated tests:
  - Backend: xUnit.
  - Frontend: use the best technology that is already standard in the repo (e.g., Jest/Vitest + Testing Library, Playwright/Cypress for e2e).
- Cover all meaningful cases for the changed functionality and update existing tests impacted by changes.
- Verify ALL tests pass successfully.
- Do not commit.

Process:

1) Pre-check and baseline
- Verify this folder is a Git repository.
- Run `git status` and note current state.
- Identify the default branch and current branch (do not switch branches).

2) Study the repository and architecture (required before coding)
- Map the solution structure:
  - Locate *.sln and list projects.
  - Identify backend projects (API/Worker/Services/Libraries) and frontend projects (apps/packages).
  - Identify existing test projects and the frameworks used (xUnit for backend, and existing frontend test stack).
- Understand architectural patterns:
  - Backend layering (Controllers/Endpoints, Application/Services, Domain, Infrastructure, Repositories).
  - Dependency injection composition root and configuration patterns.
  - Validation, error handling, logging, and observability patterns.
  - Data access approach (EF Core/Dapper/etc.), migrations and DB boundaries.
  - External integrations and how they are abstracted/mocked.
- Study previous similar work:
  - Find similar features, modules, endpoints, handlers, or components.
  - Review recent/related changes in the repo (e.g., similar task numbers, changelog, docs, code comments, TODOs).
  - Identify and reuse established conventions (naming, file placement, method patterns).
- Locate task ${1} references:
  - Search for "${1}" across code, docs, PR templates, issue references, and comments.
- Determine expected behavior / acceptance criteria:
  - Prefer explicit documentation if present.
  - Otherwise infer from existing behavior, similar features, and existing tests.
  - Document the inferred acceptance criteria in the final report.

3) Plan the change
- Identify impacted components (backend and/or frontend).
- Identify required contract changes (DTOs, API contracts, UI state, validation rules).
- Identify side effects (DB changes, caching, events, external calls).
- Identify which tests will break and which new tests must be added.
- Choose the minimum-change approach consistent with repo style.

4) Implement task ${1}
- Implement the required behavior in the correct layer(s).
- Ensure code compiles/builds.
- Keep changes scoped to the task.
- If a schema/config change is required, implement it using the repo’s standard approach.

5) Backend tests (xUnit only)
- Add or update xUnit tests for backend logic that changed.
- Use the existing backend test patterns (fixtures, factory, mocks, integration harness).
- Cover all meaningful cases, including:
  - Happy path
  - Boundary values and edge cases
  - Validation failures
  - Error paths/exceptions
  - Authorization/permission cases (if applicable)
  - Idempotency/concurrency (if relevant)
  - Mapping/serialization and contract expectations (if relevant)
- If existing tests are now outdated:
  - Fix them to align with new behavior, or
  - Expand them with additional cases.
- Ensure tests are deterministic (no real network/time unless the repo provides stable harnesses).

6) Frontend tests (best existing tech in this repo)
- Determine the frontend testing stack already used (do not introduce a new stack unless none exists).
- Add/update tests for changed UI behavior:
  - Unit/component tests (e.g., Jest/Vitest + Testing Library)
  - E2E tests (e.g., Playwright/Cypress) if the change is best validated end-to-end
- Cover cases:
  - Rendering states (loading/empty/error/success)
  - User interactions
  - Validation and error handling
  - API contract changes (mocked responses)
  - Regression cases affected by the task

7) Verify the feature runs and works
- Identify how to execute/validate the functionality:
  - Backend: run the relevant service/API/worker.
  - Frontend: run the app or targeted page/flow if applicable.
- Execute the minimal commands needed to validate end-to-end behavior.
- Validate outputs/side effects match the acceptance criteria.

8) Verify all tests pass
- Backend:
  - `dotnet test`
- Frontend:
  - Use the repo’s standard test commands (e.g., `npm test`, `pnpm test`, `yarn test`, `npm run test:e2e`, etc.).
- Fix failures caused by your changes.
- Do not ignore failing tests.

9) Final report (no git operations)
Provide a clear summary including:
- What task ${1} required (explicit or inferred acceptance criteria).
- What you changed (high-level) and where (key files/modules).
- How you verified runtime behavior (exact commands and what you observed).
- Backend tests added/updated (xUnit): test names + what each covers.
- Frontend tests added/updated: framework used + test names + what each covers.
- Commands used to run tests and confirmation that all passed.
- Any follow-up notes/risks if something could not be fully verified locally