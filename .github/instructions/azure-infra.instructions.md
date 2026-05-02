---
description: "Use when designing or configuring Azure infrastructure: App Services, Azure Functions, Blob Storage, Service Bus, Key Vault, Application Insights, Logic Apps, or Bicep/ARM templates. Covers naming, managed identity, environment config, and security."
---

## Azure Infrastructure Standards

### Naming Conventions
- Format: `<workload>-<environment>-<resource-type>[-<region>]`
- Examples: `myapp-prod-api`, `myapp-dev-func`, `myapp-staging-sb`
- Resource type abbreviations: `api` (App Service), `func` (Function App), `st` (Storage), `sb` (Service Bus), `kv` (Key Vault), `ai` (App Insights), `tbl` (Table Storage workload resources)

### Identity & Access
- All app-to-service authentication uses **managed identity** — no connection strings containing passwords or keys
- Enable system-assigned managed identity on every App Service and Function App
- Assign the minimum required RBAC role — don't use Owner or Contributor for application identities
- Common role assignments:
  - Blob Storage: `Storage Blob Data Contributor`
  - Service Bus: `Azure Service Bus Data Sender` / `Azure Service Bus Data Receiver`
  - Key Vault secrets: `Key Vault Secrets User`

### Deployment Identity Baseline (GitHub OIDC)
- For GitHub Actions infra deployment identities, require:
  - `Reader` at subscription scope (needed for reliable subscription discovery during login)
  - `Contributor` at target resource group scope (needed for group-scoped deployments)
- Treat missing either role as a preflight blocker and resolve before rerunning workflows
- OIDC subject should be environment-scoped where possible (for example: `repo:<owner>/<repo>:environment:dev`)

### Key Vault
- One Key Vault per environment — never share across environments
- App Services/Functions reference secrets via Key Vault references in app settings: `@Microsoft.KeyVault(SecretUri=...)`
- Rotate secrets via Key Vault versioning — apps pick up new versions automatically via the reference

### Azure Functions (Isolated Worker Model)
- Use the **isolated worker model** (not in-process) for all new Function Apps
- Configure in `Program.cs` with the same DI and Options patterns as the API
- Storage account for the Function App runtime is separate from the application's Blob Storage account
- Use `ServiceBusTrigger` for queue processing; `TimerTrigger` for scheduled jobs; `HttpTrigger` for lightweight HTTP endpoints that don't need the full API

### Blob Storage
- Containers named in `kebab-case` — `user-uploads`, `processed-documents`
- Enable soft delete and versioning on all storage accounts
- Use Shared Access Signatures (SAS) only for short-lived, delegated client access — never for server-to-server

### Service Bus
- Queues for point-to-point; Topics/Subscriptions for fan-out/pub-sub
- Enable dead-letter queues and set `MaxDeliveryCount` (default 10) on all queues
- Process messages idempotently — assume any message may be delivered more than once

### Application Insights
- One App Insights resource per environment
- Wire up via connection string (not instrumentation key, which is deprecated)
- Enable distributed tracing across all components — use the same App Insights resource for API, Functions, and frontend

### Bicep
- All infrastructure defined as Bicep modules — never click-ops in production
- Parameterise environment differences (`environmentName`, `location`) — no hardcoded environment values
- Run `az deployment group create --what-if` before applying infrastructure changes
- Store Bicep files in `infra/` at the repo root

### Azure Table Naming Rules
- Table names must be 3-63 characters, alphanumeric only, and must start with a letter
- Do not use hyphens or reserved names (for example `tables`)
- Keep naming casing stable even though Azure Table names are case-insensitive

### Infra Output Contract
- Every infra deployment that provisions storage for app persistence must output:
  - storage account name
  - table service endpoint URI
  - table names for each workload entity
- Downstream runtime configuration should be sourced from deployment outputs, not duplicated defaults

### Infra Preflight
- Before deploying from CI, verify:
  - required GitHub variables are present
  - parameter file exists for the selected environment
  - table names in parameters satisfy Azure Table naming rules
  - target resource group exists or is intentionally created by pipeline

### Logic Apps
- Use Consumption plan Logic Apps for low-frequency automation triggered by events (webhooks, schedules)
- Avoid Logic Apps for high-throughput, latency-sensitive flows — use Functions instead
- Store Logic App definitions in version control; deploy via ARM/Bicep
