# Implementation Plan: .NET 10 + React TypeScript Training Project

> **Status:** Draft  
> **Created:** 2026-03-01  
> **Purpose:** Educational training project — ASP.NET Core 10 Web API + React 19 (TypeScript, Vite)

---

## Table of Contents

- [1. Overview](#1-overview)
- [2. Folder Structure](#2-folder-structure)
- [3. Implementation Steps](#3-implementation-steps)
  - [3.1 Repository Setup](#31-repository-setup)
  - [3.2 Backend — ASP.NET Core 10 Web API](#32-backend--aspnet-core-10-web-api)
  - [3.3 Frontend — React 19 + TypeScript (Vite)](#33-frontend--react-19--typescript-vite)
  - [3.4 Documentation](#34-documentation)
- [4. Key Configuration Files](#4-key-configuration-files)
- [5. Running the Projects](#5-running-the-projects)
- [6. Verification](#6-verification)
- [7. Design Decisions](#7-design-decisions)

---

## 1. Overview

Create a greenfield training project consisting of:

- **Backend** — ASP.NET Core 10 Web API with a single `/health` endpoint returning HTTP 200 and a JSON status payload.
- **Frontend** — React 19 + TypeScript application bootstrapped with Vite, making a simple API call to the backend health endpoint.
- **Documentation** — Build and run guides for both backend and frontend.

The project follows conventions defined in the repository instruction files:

- C# 14, file-scoped namespaces, nullable reference types, XML doc comments on public APIs.
- React 19 functional components, hooks, strict TypeScript, modern build tooling.

---

## 2. Folder Structure

```
FTG12_Workshop_Task/
├── .github/instructions/              # (existing) Copilot instruction files
├── .gitignore                         # Combined .NET + Node ignore rules
├── tasks/
│   └── initial_task.md                # This file
├── backend/
│   ├── FTG12_ReviewsApi.slnx              # Solution file
│   └── FTG12_ReviewsApi/
│       ├── FTG12_ReviewsApi.csproj        # Project file targeting net10.0
│       ├── Program.cs                     # Application entry point
│       ├── Controllers/
│       │   └── HealthController.cs        # Health check API controller
│       ├── Models/
│       │   └── HealthStatus.cs            # Health response model (record)
│       ├── Properties/
│       │   └── launchSettings.json        # Kestrel/IIS launch profiles
│       ├── appsettings.json               # Application configuration
│       └── appsettings.Development.json
├── frontend/
│   ├── index.html                     # Vite HTML entry point
│   ├── package.json                   # npm dependencies and scripts
│   ├── tsconfig.json                  # TypeScript configuration (root)
│   ├── tsconfig.app.json              # TypeScript config for app source
│   ├── tsconfig.node.json             # TypeScript config for Vite/Node
│   ├── vite.config.ts                 # Vite configuration
│   ├── eslint.config.js               # ESLint configuration
│   ├── public/                        # Static assets
│   └── src/
│       ├── main.tsx                   # React DOM entry point
│       ├── App.tsx                    # Root component
│       ├── App.css                    # Root component styles
│       ├── hooks/
│       │   └── useHealthCheck.ts      # Custom hook for health API call
│       ├── services/
│       │   └── apiClient.ts           # HTTP client / fetch wrapper
│       └── types/
│           └── health.ts              # TypeScript interfaces for API types
└── docs/
    ├── backend-guide.md               # How to build and run the backend
    └── frontend-guide.md              # How to build and run the frontend
```

---

## 3. Implementation Steps

### 3.1 Repository Setup

| # | Task | Details |
|---|------|---------|
| 1 | Create `.gitignore` | Combine the standard GitHub `.gitignore` templates for **VisualStudio** and **Node**. Must cover: `bin/`, `obj/`, `*.user`, `*.suo`, `.vs/`, `node_modules/`, `dist/`, `.env.local`, `.env`, `*.log`. |
| 2 | Create `tasks/` directory | Place this implementation plan file at `tasks/initial_task.md`. |

### 3.2 Backend — ASP.NET Core 10 Web API

| # | Task | Details |
|---|------|---------|
| 3 | Create solution file | Run `dotnet new sln -n FTG12_ReviewsApi -o backend`. |
| 4 | Create Web API project | Run `dotnet new webapi -n FTG12_ReviewsApi -o backend/FTG12_ReviewsApi --use-controllers`. |
| 5 | Add project to solution | Run `dotnet sln backend/FTG12_ReviewsApi.slnx add backend/FTG12_ReviewsApi/FTG12_ReviewsApi.csproj`. |
| 6 | Configure `FTG12_ReviewsApi.csproj` | Target `net10.0`. Enable `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>`. Add `<AssemblyName>` and `<RootNamespace>` set to `FTG12_ReviewsApi`. |
| 7 | Implement `Program.cs` | Use top-level statements. Register controllers (`AddControllers()`). Configure CORS to allow `http://localhost:7200`. Map controllers. Optionally add `AddHealthChecks()`. Application URL (`http://localhost:7100`) is configured via `Properties/launchSettings.json` (Step 12); no explicit Kestrel URL binding is required in `Program.cs`. |
| 8 | Create `Models/HealthStatus.cs` | Define as a C# record: `public record HealthStatus(string Status, DateTime Timestamp);`. |
| 9 | Create `Controllers/HealthController.cs` | `[ApiController]`, `[Route("[controller]")]`. Single `[HttpGet]` action at `/health` returning `Ok(new HealthStatus("Healthy", DateTime.UtcNow))`. Add XML doc comments. |
| 10 | Configure `appsettings.json` | Set logging levels (`Information` default, `Warning` for Microsoft/ASP.NET). Set `AllowedHosts: "*"`. |
| 11 | Configure `appsettings.Development.json` | Override logging to `Debug` level for development. |
| 12 | Configure `launchSettings.json` | Set application URL to `http://localhost:7100`. Remove HTTPS profile for simplicity in training. |
| 13 | Remove scaffolded extras | Delete any auto-generated `WeatherForecast` controller/model from the template. |

### 3.3 Frontend — React 19 + TypeScript (Vite)

| # | Task | Details |
|---|------|---------|
| 14 | Scaffold Vite project | Run `npm create vite@latest frontend -- --template react-ts` from the repo root. |
| 15 | Install dependencies | Run `cd frontend && npm install`. |
| 16 | Define TypeScript types | Create `src/types/health.ts` with `export interface HealthStatus { status: string; timestamp: string; }`. |
| 17 | Create API client | Create `src/services/apiClient.ts`. Export an async `fetchHealth()` function using `fetch("http://localhost:7100/health")`. Parse and return typed `HealthStatus`. |
| 18 | Create custom hook | Create `src/hooks/useHealthCheck.ts`. Implement `useHealthCheck()` using `useState` + `useEffect`. Expose `{ data, loading, error }`. Call `fetchHealth()` on component mount with proper cleanup. |
| 19 | Update `App.tsx` | Minimal functional component. Call `useHealthCheck()`. Render health status, loading indicator, or error message. |
| 20 | Clean up scaffolded files | Remove default Vite demo content (counter, logos) not needed for the training project. Keep `main.tsx` entry point intact. |
| 21 | Configure ESLint | Ensure `eslint.config.js` includes TypeScript and React rules. |

### 3.4 Documentation

| # | Task | Details |
|---|------|---------|
| 22 | Create `docs/backend-guide.md` | **Prerequisites:** .NET 10 SDK. **Build:** `dotnet build` from `backend/`. **Run:** `dotnet run --project FTG12_ReviewsApi`. **Verify:** `curl http://localhost:7100/health` → `200 OK` with JSON `{ "status": "Healthy", "timestamp": "..." }`. |
| 23 | Create `docs/frontend-guide.md` | **Prerequisites:** Node.js 20.19.0+ (or 22.12.0+), npm 10+. **Install:** `npm install` from `frontend/`. **Run:** `npm run dev`. **Verify:** Open `http://localhost:7200` — page loads and displays health check result. **Build for production:** `npm run build` outputs to `dist/`. |

---

## 4. Key Configuration Files

### `backend/FTG12_ReviewsApi/FTG12_ReviewsApi.csproj`

- Target framework: `net10.0`
- Nullable reference types: enabled
- Implicit usings: enabled
- `AssemblyName` and `RootNamespace` set to `FTG12_ReviewsApi`

### `backend/FTG12_ReviewsApi/Program.cs`

- Top-level statements (no `Main` method)
- `builder.Services.AddControllers()`
- CORS policy allowing `http://localhost:7200`
- `app.MapControllers()`
- Application URL (`http://localhost:7100`) is set in `Properties/launchSettings.json`, not in `Program.cs`

### `frontend/vite.config.ts`

- Vite React plugin configuration
- Dev server on port `7200` (`strictPort: true`)

### `frontend/tsconfig.json`

- Strict mode enabled
- Target: ES2020+
- Module: ESNext
- JSX: react-jsx

---

## 5. Running the Projects

### Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project FTG12_ReviewsApi
```

The API will be available at `http://localhost:7100/health`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The app will be available at `http://localhost:7200`.

> **Note:** Start the backend first so the frontend health check call succeeds.

---

## 6. Verification

| Check | Command / Action | Expected Result |
|-------|-----------------|-----------------|
| Backend builds | `dotnet build` in `backend/` | Build succeeded, 0 warnings |
| Health endpoint | `curl http://localhost:7100/health` | `200 OK` — `{ "status": "Healthy", "timestamp": "2026-03-01T..." }` |
| Frontend builds | `npm run build` in `frontend/` | `dist/` folder created, no TypeScript errors |
| Frontend runs | Open `http://localhost:7200` | Page loads, health status displayed |
| CORS works | Frontend fetches from backend | No CORS errors in browser console |

---

## 7. Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Build tool | **Vite** over Create React App | CRA is deprecated; Vite is the modern standard for React 19 projects. |
| API style | **Controller-based** over Minimal API | Controllers provide explicit structure that is easier for learners to follow. |
| Cross-origin | **CORS** over Vite proxy | Keeps projects fully independent; CORS configuration is itself educational. |
| Folder name | **`frontend/`** | Matches the user's primary suggestion and common convention. |
| State management | **`useState` / `useEffect`** via custom hook | Sufficient for a single API call; avoids unnecessary complexity for a training project. |
| Target framework | **.NET 10** | Matches instruction file requirements; uses C# 14 features. |
| HTTPS | **Disabled** for dev | Simplifies setup for training; avoids certificate issues. |
