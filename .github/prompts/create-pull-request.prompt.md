---
description: Create a new branch and PR
---

You are working inside a VS Code workspace that is a Git repository.

The branch name is provided as the first argument:
${1}

The commit name is provided as the second argument:
${2}

If no branch name is provided or no commit name is provided, STOP and inform the user:
"Please provide a branch name. Example: /create-pr feature/login-fix"

1) Pre-check
- Verify this folder is a Git repository.
- Run `git status`.
- If there are no changes to commit, STOP and inform the user: "No changes to commit."

2) Ensure main is up to date
- `git checkout main`
- `git pull origin main`
- If pull fails, STOP and report the error.

3) Create new branch
- `git checkout -b ${1}`
- If branch already exists, STOP.

4) Stage and commit
- `git add .`
- `git commit -m "${2}"`

5) Push branch
- `git push -u origin ${1}`

6) Create PR
- If `gh` exists:
  - `gh pr create --base main --head ${1} --title "${1}" --body "Copilot created PR"`
- Otherwise output compare URL.

7) Report:
- Branch name
- Commit hash
- PR link