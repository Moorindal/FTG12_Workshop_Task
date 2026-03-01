---
description: Commit and push all current changes to the current branch
---

You are working inside a VS Code workspace that is a Git repository.

The commit message is provided as the first argument:
${1}

1) Pre-check
- Verify this folder is a Git repository.
- Run `git status`.
- If there are no changes to commit, STOP and inform the user:  
  "No changes to commit."

2) Detect current branch
- Run `git rev-parse --abbrev-ref HEAD`
- If current branch is `HEAD`, STOP and inform the user:  
  "You are in detached HEAD state. Please checkout a branch first."

3) Pull latest changes
- `git pull`
- If pull fails, STOP and report the error.

4) Stage and commit all current changes
- `git add .`
- If no commit message is provided, use:
  - `git commit -m "Update changes"`
- Otherwise use:
  - `git commit -m "${1}"`

5) Push
- `git push`

6) Report:
- Current branch name
- Commit hash
- Confirmation that push was successful