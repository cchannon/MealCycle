# Project Guidelines

## Monorepo Structure

```
/
├── frontend/          # Vite + React + TypeScript SPA
├── backend/           # C# ASP.NET Core Web API / Azure Functions
├── .github/
│   ├── agents/        # Lifecycle persona agents
│   ├── instructions/  # Conditional file instructions
│   ├── prompts/       # Reusable slash-command templates
│   ├── skills/        # Tech stack specialist skills
│   └── workflows/     # GitHub Actions CI/CD pipelines
└── .vscode/
    └── mcp.json       # MCP server configuration
```

Folder-level `AGENTS.md` files in `frontend/` and `backend/` provide area-specific overrides. The closest file to the edited file takes precedence.

## Shared Conventions

- All secrets and connection strings are stored in Azure Key Vault; apps resolve them via managed identity at runtime — never hardcode credentials
- Environment promotion follows: `dev` → `staging` → `production`; no direct deployments to production
- Feature branches off `main`; PRs require at least one reviewer and a passing CI run before merge
- Commit messages follow Conventional Commits: `feat:`, `fix:`, `chore:`, `docs:`, `test:`, `refactor:`

## Testing Philosophy

- Every public method and API endpoint has unit tests before it reaches a staging environment
- UI regression is covered by Playwright E2E tests that run on every PR targeting `main`
- Test naming: describe the behaviour, not the implementation — `"returns 400 when email is missing"` not `"TestValidation"`

## External Integrations

### Authentication
- Microsoft Entra ID (OIDC/OAuth 2.0) is the identity provider for this internal app
- Frontend acquires Entra tokens and calls backend APIs with bearer tokens
- Backend validates Entra-issued JWTs on protected API routes

### Data Storage
- Azure Storage Account (Table Storage) is the primary persistence layer for application data
- Prefer cost-conscious schema and query patterns designed for partition/row key access

## Observability

- Application Insights is the telemetry sink at every tier (frontend JS SDK, backend .NET SDK, Azure Functions)
- Every significant operation logs a structured event with a correlation ID that flows end-to-end
- Errors are tracked with full exception context; never swallow exceptions silently

## Source Management & CI/CD

- GitHub for source control; GitHub Actions for all CI/CD pipelines
- Workflows live in `.github/workflows/`; each environment has its own deployment workflow
- Azure environments are provisioned via Bicep; infrastructure changes go through PR review like code

### Deployment Autopilot
- Assume infra deployment to `dev` is an autonomous operation and execute it end-to-end unless a hard blocker occurs
- Run this preflight before infra deployment attempts:
    - verify active GitHub auth identity is expected for the target repository
    - verify workflow exists on remote and can be triggered by the chosen path
    - verify Azure tenant and subscription context match deployment scope
    - verify deployment service principal has required RBAC (subscription Reader, resource group Contributor)
- Prefer non-interactive command flows for run monitoring and diagnostics; avoid commands that may require terminal input when a polling alternative exists
- If local, user-approved workspace changes are already acknowledged in the current thread, continue without re-prompting on those same files
- If workflow dispatch or rerun permissions are missing, use the documented push-trigger path for infra workflows instead of stopping

## Design Process

- Functional requirements and architecture are detailed upfront before implementation begins
- Architecture Decision Records (ADRs) document significant technical choices
- Technical specifications are produced iteratively during agile sprints, not upfront
