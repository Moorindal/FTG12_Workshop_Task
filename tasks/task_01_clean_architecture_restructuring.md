# Task 01 — Clean Architecture Project Restructuring

## Goal
Restructure the existing single-project backend solution into a Clean Architecture layout with four projects: Domain, Application, Infrastructure, and Web/API.

## Scope
- Create three new class library projects: `FTG12_ReviewsApi.Domain`, `FTG12_ReviewsApi.Application`, `FTG12_ReviewsApi.Infrastructure`.
- Move the existing Web/API project (`FTG12_ReviewsApi`) into `backend/src/FTG12_ReviewsApi/`.
- Organize all projects under `backend/src/`.
- Set up correct project references (dependency flow: Web → Infrastructure → Application → Domain).
- Update the solution file to include all projects.
- Ensure the existing `/health` endpoint still works after restructuring.

## Acceptance Criteria
- [ ] Solution contains four projects: Domain, Application, Infrastructure, Web/API.
- [ ] Project references enforce Clean Architecture dependency rules:
  - Domain references nothing.
  - Application references Domain only.
  - Infrastructure references Application and Domain.
  - Web/API references Application and Infrastructure.
- [ ] All projects target `net10.0` with nullable reference types enabled.
- [ ] `dotnet build` succeeds with zero errors.
- [ ] `dotnet run --project src/FTG12_ReviewsApi` starts the API.
- [ ] `GET /health` returns `200 OK` with the existing health status JSON.
- [ ] Each project has a `DependencyInjection.cs` extension method class (Application and Infrastructure) for registering services.

## Notes / Edge Cases
- The existing `HealthController` and `HealthStatus` model remain in the Web/API project for now (health is an infrastructure concern, not a domain feature).
- Do not add any NuGet packages yet — that happens in subsequent tasks.
- Ensure `Properties/launchSettings.json` and `appsettings*.json` remain in the Web/API project.
- File-scoped namespaces must be used in all new files (C# 14 convention).
- Update CORS policy if the Web project path changes.

## Dependencies
- None — this is the first task.

## Testing Notes
- Manual: `dotnet build` + `dotnet run` + `curl http://localhost:7100/health`.
- No automated tests yet (test projects are created in Tasks 12–13).
