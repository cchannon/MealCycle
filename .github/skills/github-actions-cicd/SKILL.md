---
name: github-actions-cicd
description: "GitHub Actions CI/CD pipeline patterns for this stack. Use when creating or modifying workflows for building, testing, and deploying C# APIs, Azure Functions, React frontends, and Bicep infrastructure to Azure. Triggers: GitHub Actions, CI/CD, workflow, pipeline, build workflow, deployment workflow, deploy to Azure, Bicep deployment, OIDC, Playwright CI, release workflow."
---

# GitHub Actions CI/CD Patterns

## When to Use
- Creating a new build, test, or deployment workflow
- Adding Playwright E2E tests to a PR pipeline
- Setting up OIDC authentication with Azure (no service principal secrets)
- Writing a Bicep infrastructure deployment workflow
- Configuring environment-gated production deployments

## Workflow Structure

This stack uses separate, focused workflows:

| Workflow File | Trigger | Purpose |
|---|---|---|
| `ci.yml` | `pull_request` → `main` | Build + unit test + lint (frontend and backend) |
| `e2e.yml` | `pull_request` → `main` | Playwright E2E against a preview/dev environment |
| `deploy-dev.yml` | Push to `main` | Deploy API + frontend to `dev` |
| `deploy-staging.yml` | Release tag or manual | Deploy to `staging` (requires test pass) |
| `deploy-production.yml` | Manual with approval | Deploy to `production` (requires staging success) |
| `infra.yml` | PR or manual | Bicep what-if on PR, apply on merge to `main` |

## CI Workflow Template

```yaml
name: CI

on:
  pull_request:
    branches: [main]

jobs:
  backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.x'
      - run: dotnet restore backend/ --locked-mode
      - run: dotnet build backend/ --no-restore --configuration Release
      - run: dotnet test backend/ --no-build --configuration Release --logger trx
      - uses: dorny/test-reporter@v1
        if: always()
        with:
          name: .NET Test Results
          path: '**/*.trx'
          reporter: dotnet-trx

  frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: frontend/package-lock.json
      - run: npm ci --prefix frontend
      - run: npm run build --prefix frontend
      - run: npm run test:ci --prefix frontend
```

## Azure OIDC Authentication

```yaml
permissions:
  id-token: write
  contents: read

steps:
  - uses: azure/login@v2
    with:
      client-id: ${{ vars.AZURE_CLIENT_ID }}
      tenant-id: ${{ vars.AZURE_TENANT_ID }}
      subscription-id: ${{ vars.AZURE_SUBSCRIPTION_ID }}
```

- `AZURE_CLIENT_ID` and friends are environment **variables** (non-secret), not secrets
- The app registration needs a federated identity credential for the GitHub repo + environment

## Environment-Gated Deployment

```yaml
deploy-production:
  needs: [deploy-staging]
  runs-on: ubuntu-latest
  environment: production  # triggers GitHub protection rules (required reviewers)
  steps:
    - uses: azure/login@v2
      with:
        client-id: ${{ vars.AZURE_CLIENT_ID }}
        tenant-id: ${{ vars.AZURE_TENANT_ID }}
        subscription-id: ${{ vars.AZURE_SUBSCRIPTION_ID }}
    - uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ vars.API_APP_NAME }}
        package: ./publish/api
```

## Playwright in CI

```yaml
playwright:
  runs-on: ubuntu-latest
  steps:
    - uses: actions/checkout@v4
    - uses: actions/setup-node@v4
      with:
        node-version: '20'
        cache: 'npm'
        cache-dependency-path: frontend/package-lock.json
    - run: npm ci --prefix frontend
    - run: npx playwright install --with-deps chromium
    - run: npx playwright test
      working-directory: frontend
      env:
        PLAYWRIGHT_BASE_URL: ${{ vars.DEV_BASE_URL }}
    - uses: actions/upload-artifact@v4
      if: failure()
      with:
        name: playwright-report
        path: frontend/playwright-report/
```

## References
- [Full deploy-dev workflow template](./references/deploy-dev-workflow.md)
- [Bicep infra workflow template](./references/infra-workflow.md)
- [OIDC federation setup steps](./references/oidc-setup.md)
