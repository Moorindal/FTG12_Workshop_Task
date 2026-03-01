---
description: Verify application health
---

You are working inside a VS Code workspace that is a Git repository containing one or more backend (.NET) and possibly frontend projects.

Goal:
- Verify that the entire solution builds successfully.
- Run all automated tests in all projects.
- Confirm that tests pass without errors.
- Run the application.
- Confirm that the application starts and runs without runtime errors.

Rules:
- Do not modify any source files.
- Do not refactor or reformat code.
- Do not create branches.
- Do not commit.
- Only build, run, and verify.

Process:

1) Repository validation
- Verify this folder is a Git repository.
- Run `git status` to ensure no unexpected state issues.

2) Build verification
- For .NET projects:
  - Run `dotnet build`
- If frontend exists:
  - Install dependencies if needed (npm/pnpm/yarn install).
  - Run project build command (e.g., npm run build).
- If any build fails, STOP and report detailed errors.

3) Run all tests
- Backend:
  - Run `dotnet test`
- Frontend:
  - Detect test framework used in the repo (Jest, Vitest, etc.).
  - Run the standard test command from package.json.
- Confirm:
  - All tests executed.
  - No failed tests.
  - No skipped tests unless expected.
- If any test fails, report:
  - Project name
  - Test name
  - Error message/stack trace.

4) Run the application
- Identify the main runnable project (API, WebApp, Worker, etc.).
- Start the backend:
  - `dotnet run --project <main-project>`
- If frontend exists:
  - Start frontend dev server using the repo’s standard command.
- Verify:
  - Application starts successfully.
  - No unhandled exceptions in logs.
  - No immediate crashes.
  - If API: confirm server is listening on expected port.
  - If Web UI: confirm server starts and serves without fatal errors.

5) Basic runtime validation
- Check console output for:
  - Errors
  - Critical warnings
  - Startup failures
- If health endpoint exists (e.g., /health), call it.
- Confirm expected startup message/log is present.

6) Final report
Provide a structured summary including:
- Build result (success/failure).
- Backend test result (total tests, passed/failed).
- Frontend test result (if applicable).
- Application startup result.
- Any runtime warnings or issues detected.
- Clear final status:
  - ✅ Application is healthy
  - ❌ Issues detected (with details)