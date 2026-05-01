---
description: "Use when writing or reviewing GitHub Actions workflow files. Covers secrets, environment targeting, job structure, caching, and deployment patterns for our Azure-hosted stack."
applyTo: ".github/workflows/**"
---

## GitHub Actions Standards

### Secrets & Environment Variables
- Never hardcode credentials — all secrets referenced via `${{ secrets.SECRET_NAME }}`
- Environment-specific secrets stored per GitHub Environment (`dev`, `staging`, `production`) — use `environment:` on deployment jobs to gate access
- Azure credentials use OIDC federation (`azure/login@v2` with `client-id`, `tenant-id`, `subscription-id`) — never a service principal password secret

### Job Structure
- Separate jobs by concern: `build`, `test`, `lint`, `deploy-dev`, `deploy-staging`, `deploy-production`
- Deployment jobs depend on (`needs:`) successful build and test jobs
- Production deployments require manual approval — configure via GitHub Environment protection rules

### Triggers
- PR workflows: `on: pull_request` targeting `main` — run build + test + lint
- Push to `main`: run build + test, then deploy to `dev`
- Release/tag: deploy to `staging` then `production` (with approval gate)

### Caching
- Cache NuGet packages: `actions/cache` keyed on `**/packages.lock.json` hash
- Cache npm/pnpm: `actions/setup-node` built-in cache, or `actions/cache` keyed on lockfile hash
- Cache Docker layers when building container images

### .NET / Backend Jobs
```yaml
- uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '8.x'
- run: dotnet restore --locked-mode
- run: dotnet build --no-restore --configuration Release
- run: dotnet test --no-build --configuration Release --logger trx
```
- Always `--locked-mode` on restore to enforce lockfile
- Publish test results via `dorny/test-reporter` or equivalent

### Frontend Jobs
```yaml
- uses: actions/setup-node@v4
  with:
    node-version: '20'
    cache: 'npm'
- run: npm ci
- run: npm run build
- run: npm run test:ci
```
- `npm ci` not `npm install` in CI — enforce lockfile

### Azure Deployment
- Use `azure/login@v2` with OIDC — no client secrets in workflows
- Static frontend: deploy to Azure Static Web Apps via `azure/static-web-apps-deploy@v1`
- API: deploy to Azure App Service via `azure/webapps-deploy@v2` or Azure Functions via `azure/functions-action@v1`
- Bicep infrastructure changes: `az deployment group create --confirm-with-what-if` in a separate infra workflow

### Playwright in CI
- Install browsers: `npx playwright install --with-deps chromium`
- Run: `npx playwright test --reporter=github`
- Upload artifacts on failure: `actions/upload-artifact` for `playwright-report/` and `test-results/`
