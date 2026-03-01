# Task 03 — MediatR + Validation Pipeline + Error Handling Middleware

## Goal
Set up MediatR as the mediator for all application use-cases, add a FluentValidation pipeline behavior, and implement a global exception handling middleware that returns consistent RFC 9457 Problem Details responses.

## Scope

### MediatR setup
- Add NuGet packages: `MediatR` (to Application), `FluentValidation`, `FluentValidation.DependencyInjection` (to Application).
- Register MediatR in `Application/DependencyInjection.cs` with assembly scanning.
- Register FluentValidation validators in `Application/DependencyInjection.cs`.

### Validation pipeline
- Create `ValidationBehavior<TRequest, TResponse>` in `Application/Common/Behaviors/`.
- The behavior collects all `IValidator<TRequest>` instances, runs them, and throws `ValidationException` if any failures occur.

### Custom exceptions
- Create in `Application/Common/Exceptions/`:
  - `ValidationException` — carries a dictionary of field → error messages.
  - `NotFoundException` — entity name + key.
  - `ForbiddenException` — message.
  - `ConflictException` — message.

### Error handling middleware
- Create `ExceptionHandlingMiddleware` in the Web/API project under `Middleware/`.
- Catches all exceptions and maps them to Problem Details JSON responses.
- Register in `Program.cs` pipeline.

## Acceptance Criteria
- [ ] MediatR is registered and resolves handlers from the Application assembly.
- [ ] FluentValidation validators are auto-registered from the Application assembly.
- [ ] `ValidationBehavior` runs before every handler and throws `ValidationException` on failures.
- [ ] Custom exception classes exist: `ValidationException`, `NotFoundException`, `ForbiddenException`, `ConflictException`.
- [ ] `ExceptionHandlingMiddleware` maps exceptions to correct HTTP status codes:
  - `ValidationException` → 400 with `errors` dictionary.
  - `NotFoundException` → 404.
  - `ForbiddenException` → 403.
  - `ConflictException` → 409.
  - `UnauthorizedAccessException` → 401.
  - Unhandled → 500 (no stack trace in response).
- [ ] All error responses follow RFC 9457 Problem Details format (`type`, `title`, `status`, `detail`, optional `errors`).
- [ ] `dotnet build` succeeds.
- [ ] Existing `/health` endpoint still works.

## Notes / Edge Cases
- The `ValidationBehavior` should gracefully handle the case where no validators exist for a given request (pass through without error).
- The 500 response should log the full exception details but only return a generic "An unexpected error occurred" message to the client.
- Use `System.Text.Json` for serialization (consistent with ASP.NET Core defaults).
- Do not add `[ApiController]` model validation behavior — let FluentValidation handle it via the MediatR pipeline instead. Disable the default `ModelStateInvalidFilter` if needed.

## Dependencies
- Task 01 (Clean Architecture project structure).

## Testing Notes
- Manual: Trigger errors by sending invalid requests (once endpoints exist).
- Automated: Exception mapping logic and `ValidationBehavior` are prime candidates for unit tests (Task 14).
- Verify the middleware does not swallow the `OperationCanceledException` (let it propagate normally for cancelled requests).
