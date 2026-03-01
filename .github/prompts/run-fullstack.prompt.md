---
description: Test app run
---

You are working inside a VS Code workspace with `backend/` (.NET) and `frontend/` (Vite/React).

When executing this prompt, follow this strict procedure:

1) Backend restore + build
- Open a new VS Code terminal named `backend`.
- In `${workspaceFolder}/backend`, run:
  - `dotnet restore`
  - `dotnet build`
- If restore/build fails, STOP immediately:
  - Copy the relevant error output.
  - Provide a short diagnosis and suggested fix.
  - Do NOT continue to backend run or frontend.

2) Backend start
- In the same `backend` terminal, run:
  - `dotnet run`
- Wait until the backend prints a successful startup message (e.g., "Now listening on:" or similar).
- If build/run fails OR no successful startup message appears, STOP immediately:
  - Copy the relevant error output.
  - Provide a short diagnosis and suggested fix.
  - Do NOT continue to frontend.
  - Do NOT run cleanup unless explicitly instructed.

3) Frontend install packages + build
- Open a second VS Code terminal named `frontend`.
- In `${workspaceFolder}/frontend`, ensure all missing packages are downloaded:
  - If `node_modules` is missing OR packages are out of date, run `npm install`
  - (Optional but recommended for CI-like consistency) If `package-lock.json` exists, prefer `npm ci` instead of `npm install`.
- Then run:
  - `npm run build`
- If install/build fails, STOP immediately:
  - Copy the relevant error output.
  - Provide a short diagnosis and suggested fix.
  - Do NOT continue to `npm run dev`.
  - Do NOT run cleanup unless explicitly instructed.

4) Frontend start (backend must keep running)
- In the same `frontend` terminal, run:
  - `npm run dev`
- Wait until Vite prints a successful startup message (e.g., "Local:" with a URL).
- If frontend fails, STOP:
  - Copy the relevant error output + suggested fix.
  - Do NOT run cleanup unless explicitly instructed.

5) Verification
- Confirm both are running:
  - Backend terminal is still running without exiting.
  - Frontend terminal is still running and shows the dev server URL.
- Provide quick sanity info (best-effort):
  - Backend URL/port from the "Now listening on:" line(s).
  - Frontend URL/port from the Vite "Local:" line.

6) Reporting (final response format)
Return a report with:
- Backend: ✅/❌, restore log snippet, build log snippet, startup log snippet, URL/port, terminal name
- Frontend: ✅/❌, install log snippet (if ran), build log snippet, startup log snippet, URL/port, terminal name
- Next steps (if any)

7) Cleanup (must happen last; stop everything started by this run)
- After reporting, stop both servers that were started by this prompt:
  - Send Ctrl+C in `frontend` terminal to stop Vite.
  - Send Ctrl+C in `backend` terminal to stop `dotnet run`.
- Confirm both terminals are no longer running the servers (they should return to a prompt / exit).
- If Ctrl+C does not stop a process, terminate the terminal/process as a fallback.
- Briefly state what was stopped.