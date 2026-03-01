# Backend — Build and Run Guide

This guide describes how to build and run the ASP.NET Core 10 Web API backend.

---

## Prerequisites

| Requirement | Version | Verify |
|------------|---------|--------|
| .NET SDK | 10.0 or later | `dotnet --version` |

Download the .NET 10 SDK from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

---

## Project Location

All backend files are in the `backend/` folder at the repository root:

```
backend/
├── FTG12_ReviewsApi.slnx              # Solution file
└── FTG12_ReviewsApi/
    ├── FTG12_ReviewsApi.csproj         # Project file (net10.0)
    ├── Program.cs                       # Application entry point
    ├── Controllers/
    │   └── HealthController.cs          # Health check endpoint
    ├── Models/
    │   └── HealthStatus.cs              # Response model
    ├── Properties/
    │   └── launchSettings.json          # Launch profiles
    ├── appsettings.json
    └── appsettings.Development.json
```

---

## Build

From the repository root:

```bash
cd backend
dotnet restore
dotnet build
```

**Expected output:**

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## Run

```bash
cd backend
dotnet run --project FTG12_ReviewsApi
```

The API starts on `http://localhost:7100`.

**Expected console output:**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:7100
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

## Verify

### Health Endpoint

```bash
curl http://localhost:7100/health
```

**Expected response (HTTP 200):**

```json
{
  "status": "Healthy",
  "timestamp": "2026-03-01T12:00:00.0000000Z"
}
```

### Built-in Health Check

ASP.NET Core's built-in health check is also available:

```bash
curl http://localhost:7100/healthz
```

**Expected response (HTTP 200):**

```
Healthy
```

---

## Configuration

### Application URLs

Configured in `Properties/launchSettings.json`. Default: `http://localhost:7100`.

### CORS

The backend allows cross-origin requests from `http://localhost:7200` (the frontend dev server). This is configured in `Program.cs`.

### Logging

| Environment | Default Level | ASP.NET Level |
|------------|---------------|---------------|
| Production | Information | Warning |
| Development | Debug | Information |

Logging configuration is in `appsettings.json` and `appsettings.Development.json`.

---

## Stopping the Server

Press `Ctrl+C` in the terminal where the server is running.
