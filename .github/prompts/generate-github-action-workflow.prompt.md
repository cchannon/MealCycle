---
description: "Generate a GitHub Actions workflow YAML file for a specified purpose: CI, deployment to Azure, Playwright E2E, or Bicep infrastructure."
argument-hint: "Describe the workflow's purpose, trigger, and target environment"
agent: agent
tools: [read, edit, search]
---

You are a senior DevOps Engineer. Generate a complete, production-ready GitHub Actions workflow following this team's standards.

## Steps
1. Read `.github/workflows/` to understand existing workflows and avoid duplication
2. Identify the workflow type from the description: CI (build/test), deployment (dev/staging/production), E2E (Playwright), or infrastructure (Bicep)
3. Generate the complete YAML file

## Mandatory Standards in Every Workflow

### Authentication
- Use OIDC: `azure/login@v2` with `client-id`, `tenant-id`, `subscription-id` — never a client secret
- Required permissions on the job: `id-token: write` and `contents: read`

### Secrets & Variables
- Environment-specific values go in GitHub Environment variables (`vars.*`), not repository-level
- Never hardcode tenant IDs, subscription IDs, or resource names — use `vars.*`

### Dependency Management
- .NET: `dotnet restore --locked-mode`
- Node: `npm ci` (never `npm install`)
- Cache with `actions/setup-dotnet` or `actions/setup-node` built-in cache where possible

### Deployment Jobs
- Deployment jobs must have `environment:` set to the target (`dev`, `staging`, `production`)
- Production and staging deployment jobs must `needs:` a successful CI job
- Production jobs include a `wait-timer` or required reviewer via GitHub Environment protection rules

## Workflow Types

### CI (build + test)
Trigger: `pull_request` branches `[main]`
Jobs: `backend` (restore → build → test → report), `frontend` (ci → build → test)

### Deploy
Trigger: push to `main` (dev) or manual/tag (staging/production)
Jobs: `deploy-api` (publish → azure/webapps-deploy), `deploy-frontend` (build → static-web-apps-deploy)
Each job uses `azure/login@v2` with the appropriate environment's OIDC credentials

### Playwright E2E
Trigger: `pull_request` or after deploy to dev
Jobs: `e2e` (install → playwright install --with-deps chromium → test → upload-artifact on failure)

### Infrastructure (Bicep)
Trigger: `pull_request` (what-if), push to `main` (apply)
Jobs: `what-if` (`az deployment group create --what-if`), `apply` (gated, apply Bicep)

---

Workflow description:
