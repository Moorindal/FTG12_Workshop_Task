---
description: Create task template file
---

You are working inside a VS Code workspace that is a Git repository.

The task number is provided as the first argument:
${1}

The task slug (short kebab-case name for filename) is provided as the second argument:
${2}

The full task description is provided as the third argument:
${3}

If any of the arguments are missing, STOP and inform the user:
"Please provide: task number, slug, and full description. Example: /create-task 1 Initial_task \"Full task description here...\""

Rules:
- Do not modify any existing files (except creating the new task file).
- Do not commit.
- Do not create branches.
- Use clean and structured Markdown.
- Keep content professional and well-formatted.
- Do not rewrite the provided description — insert it as-is into the appropriate section.

1) Pre-check
- Verify this folder is a Git repository.
- Ensure `/tasks` directory exists.
  - If not, create it.

2) Create task file
- Create (or overwrite) the file:
  `/tasks/${1}-${2}.md`

3) Populate file using the following template:

# Task #${1} – ${2}

## Description
${3}

---

## Scope

### Backend
- Describe required backend changes.
- Describe required handlers/services.

### Frontend
- Describe UI changes according to business rules.
- Ensure role-based rendering logic is centralized.

---

## Testing Requirements

### Backend Tests (xUnit)
- Describe update tests for new behavior.
- Cover happy paths, edge cases, and error scenarios.
- Ensure correct HTTP status codes and validation messages.

### Frontend Tests
- Add or update UI tests using the existing frontend test framework.
- Verify conditional rendering and interaction behavior.
- Ensure regular user flows remain unaffected.

---

## Acceptance Criteria
- All business rules described above are implemented.

---

## Definition of Done
- Task requirements fully implemented.

4) Verify
- Confirm file exists at `/tasks/${1}-${2}.md`.
- Confirm content includes:
  - Task number
  - Slug
  - Provided description

5) Report
- Provide created file path.
- Confirm no other files were modified.