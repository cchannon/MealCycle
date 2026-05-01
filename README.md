# MealCycle

MealCycle is a tablet-first and phone-optimized meal planning app with a C# backend and React frontend.

## Current Implementation Baseline

This first implementation slice includes:
- Backend solution scaffolded with API, Application, Domain, Infrastructure, and test projects
- Recipe vertical slice with in-memory persistence
- API endpoints:
  - `GET /api/recipes`
  - `GET /api/recipes/{recipeId}`
  - `POST /api/recipes`
  - `GET /health`
- Frontend routed shell with pages for:
  - Recipe library
  - Meal plan board (drag and drop)
  - Shopping list preview workflow
  - Cook mode preview
  - AI tools roadmap page
- Recipe library form + list wired to backend API
- Infrastructure prep scaffold under `infra/` with Table Storage Bicep and environment parameters
- Backend config placeholders for Azure Table Storage and Foundry model settings
- AI Tools page readiness panel backed by `/health`

## Run Locally

### Backend

```powershell
Set-Location backend/src/Api
dotnet run
```

The API will run with HTTPS and expose OpenAPI in development.

### Frontend

```powershell
Set-Location frontend
npm install
npm run dev
```

Optionally set API URL in `frontend/.env.local`:

```bash
VITE_API_BASE_URL=http://localhost:5186
```

## Validate

```powershell
Set-Location backend
dotnet build MealCycle.sln
dotnet test MealCycle.sln

Set-Location ../frontend
npm run build
```

## Infrastructure Setup (Azure + GitHub Actions)

This repository is prepared to deploy baseline infrastructure from `infra/main.bicep` using GitHub Actions.
These steps are written for public, anonymous users who want to replicate this setup in their own Azure tenant/subscription.

### 1. Prerequisites

1. Azure subscription and target resource groups for `dev`, `staging`, and `production`.
2. GitHub repository with Environments configured for `staging` and `production` approval gates.
3. Azure CLI installed locally for one-time identity setup and optional local validation.

### 2. Configure Azure OIDC for GitHub Actions (recommended)

1. Create an Azure app registration/service principal for CI/CD.
2. Add federated credentials scoped to this GitHub repository/workflow.
3. Grant least-privilege role assignments at the target scope (typically `Contributor` on each environment resource group).

Capture these values from Azure:
- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

How to determine these values:
1. In Azure Portal:
  - `AZURE_CLIENT_ID`: App registration -> Overview -> Application (client) ID.
  - `AZURE_TENANT_ID`: App registration -> Overview -> Directory (tenant) ID.
2. In Azure CLI:

```powershell
az account show --query "{subscriptionId:id, tenantId:tenantId}" -o table
```

Use the `id` value as `AZURE_SUBSCRIPTION_ID`.

### 3. Configure GitHub Actions variables/secrets

Use repository-level variables for non-sensitive values and environment-level values where they differ by environment.

Recommended repository/environment variables:
- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_LOCATION`
- `AZURE_RESOURCE_GROUP`
- Optional runtime target variables (set one or both):
  - `AZURE_WEBAPP_NAME`
  - `AZURE_FUNCTIONAPP_NAME`
- `WORKLOAD_NAME` (example: `mealcycle`)
- Optional workflow environment-name overrides:
  - `DEPLOY_ENV_DEV` (default `dev`)
  - `DEPLOY_ENV_STAGING` (default `staging`)
  - `DEPLOY_ENV_PRODUCTION` (default `production`)

Sensitive values (if any) should be stored in:
- GitHub environment secrets (temporary bootstrap), or preferably
- Azure Key Vault referenced by deployed applications.

How to choose/populate common variables:
1. `AZURE_LOCATION`: choose an Azure region available in your subscription (for example `eastus`, `westeurope`).
2. `AZURE_RESOURCE_GROUP`: create/select a resource group that will hold your infrastructure deployment.
3. `WORKLOAD_NAME`: short lowercase name for your workload (for example `mealcycle`, `mymealapp`).

Create a resource group (if needed):

```powershell
az group create --name <resource-group-name> --location <azure-region>
```

Set variables in GitHub:
1. Repository -> Settings -> Secrets and variables -> Actions -> Variables.
2. For environment-specific values, use:
  - Repository -> Settings -> Environments -> <environment> -> Variables.

### 4. Parameter files

1. Keep environment-specific files in `infra/`.
2. Existing development file: `infra/main.dev.parameters.json`.
3. Staging and production files are included:
  - `infra/main.staging.parameters.json`
  - `infra/main.production.parameters.json`

### 5. CI/CD deployment flow

Recommended workflow pattern:
1. Run build/test CI on every PR.
2. On merge to `main`, deploy `dev` automatically.
3. Run `what-if` before `create` for every infra deployment.
4. Promote to `staging` and `production` using protected GitHub Environments and manual approval.

Workflow file:
- `.github/workflows/infra-deploy.yml`

Deployment output handling:
1. The infra workflow captures `az deployment group create` JSON output to `deployment-result.json`.
2. Key outputs are extracted and exposed in the job summary and job outputs:
  - `storageAccountName`
  - `storageTableEndpoint`
  - `recipesTableName`
  - `mealPlanTableName`
  - `cookProgressTableName`
3. The full deployment JSON is uploaded as a workflow artifact for downstream jobs.

This is the bridge to persistence rollout:
1. The infra workflow now includes an automatic runtime-configuration step.
2. When `AZURE_WEBAPP_NAME` and or `AZURE_FUNCTIONAPP_NAME` is configured, the workflow writes runtime settings automatically:
  - `Persistence__Provider`
  - `AzureStorage__TableServiceUri`
  - `AzureStorage__RecipesTableName`
  - `AzureStorage__MealPlanTableName`
  - `AzureStorage__CookProgressTableName`
3. If neither runtime target variable is configured, deployment still succeeds and the workflow logs that runtime settings were skipped.

Suggested deployment command in workflow jobs:

```powershell
az deployment group create `
  --resource-group <resource-group> `
  --template-file infra/main.bicep `
  --parameters @infra/main.<environment>.parameters.json
```

And preflight diff step:

```powershell
az deployment group what-if `
  --resource-group <resource-group> `
  --template-file infra/main.bicep `
  --parameters @infra/main.<environment>.parameters.json
```

### 6. Security and operations notes

1. Do not commit credentials, connection strings, or API keys.
2. Prefer managed identity + Key Vault for runtime secrets.
3. Keep environment promotion order strict: `dev` -> `staging` -> `production`.
4. Treat Bicep changes as code changes requiring PR review.

## Next Implementation Steps

1. Provision Storage Account + Table resources from `infra/main.bicep`.
2. Provision Foundry project/model deployments and bind configuration values.
3. Replace in-memory repositories with Azure Table Storage repositories.
4. Integrate Foundry-backed AI Tools backend endpoints.
5. Add Entra auth and correlation-aware telemetry plumbing.
6. Add Playwright tablet and phone journey tests.
