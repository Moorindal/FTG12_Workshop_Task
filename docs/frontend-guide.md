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

## Project Location

All frontend files are in the `frontend/` folder at the repository root:

```
frontend/
├── index.html              # Vite HTML entry point
├── package.json            # Dependencies and scripts
├── vite.config.ts          # Vite configuration
├── tsconfig.json           # TypeScript configuration
├── eslint.config.js        # ESLint rules
├── public/                 # Static assets
└── src/
    ├── main.tsx            # React DOM entry point
    ├── App.tsx             # Root component
    ├── App.css             # Root component styles
    ├── index.css           # Global styles
    ├── hooks/
    │   └── useHealthCheck.ts   # Custom health check hook
    ├── services/
    │   └── apiClient.ts        # Backend API client
    └── types/
        └── health.ts           # TypeScript interfaces
```

---

## Install Dependencies

From the repository root:

```bash
cd frontend
npm install
```

**Expected output:**

```
added XXX packages, and audited XXX packages in Xs

found 0 vulnerabilities
```

---

## Run (Development)

```bash
cd frontend
npm run dev
```

The dev server starts on `http://localhost:7200` with hot module replacement (HMR).

**Expected console output:**

```
  VITE v7.x.x  ready in XXX ms

  ➜  Local:   http://localhost:7200/
  ➜  Network: use --host to expose
  ➜  press h + enter to show help
```

---

## Build (Production)

```bash
cd frontend
npm run build
```

Outputs optimized static files to the `dist/` folder.

**Expected output:**

```
vite v7.x.x building for production...
✓ XX modules transformed.
dist/index.html           ...
dist/assets/index-xxx.js  ...
dist/assets/index-xxx.css ...
✓ built in Xs
```

---

## Verify

1. **Start the backend first** — The frontend calls `http://localhost:7100/health`.  
   See [backend-guide.md](backend-guide.md) for instructions.

2. **Open the app** — Navigate to `http://localhost:7200` in your browser.

3. **Expected result:**
   - The page displays **"Training Project"** as the heading.
   - The **Backend Health Check** card shows:
     - **Status:** Healthy (in green)
     - **Timestamp:** The current UTC date/time
   - If the backend is not running, the card shows an error message in red.

---

## Linting

```bash
cd frontend
npm run lint
```

Runs ESLint with TypeScript and React rules.

---

## Key Configuration

### API Base URL

The backend URL is defined in `src/services/apiClient.ts`:

```typescript
const API_BASE_URL = 'http://localhost:7100';
```

Update this value if the backend runs on a different port.

### TypeScript

Strict mode is enabled. Configuration is split across:

- `tsconfig.json` — Root references
- `tsconfig.app.json` — App source compilation settings
- `tsconfig.node.json` — Vite/Node tooling settings

### Vite

Configured in `vite.config.ts` with the React plugin. Dev server runs on port `7200` (`strictPort: true`).

---

## Stopping the Dev Server

Press `Ctrl+C` in the terminal where the dev server is running, or press `q + Enter`.
