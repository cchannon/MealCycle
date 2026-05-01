# Azure Infrastructure Prep (Stop Point)

This folder contains pre-provisioning scaffolding so we can discuss and confirm infrastructure choices before creating cloud resources.

<!-- Trigger note: workflow retry via infra path change -->

## Scope prepared

- Azure Storage Account design for Table Storage-backed persistence
- Table names for recipes, meal plan, and cook progress
- Foundry model integration configuration contract (endpoint/deployment names)

## What is intentionally NOT done yet

- No Azure resources are provisioned
- No Foundry model deployments are created
- No production credentials or Key Vault references are configured

## Naming convention

Use `<workload>-<environment>-<resource-type>[-<region>]`.

Examples:

- `mealcycle-dev-st`
- `mealcycle-dev-ai`

## Preflight checklist before provisioning

1. Confirm target subscription, resource group, and location.
2. Confirm environment names (`dev`, `staging`, `production`).
3. Confirm managed identity strategy for API/Functions.
4. Confirm Storage Table names and retention policy.
5. Confirm Foundry deployment names for model inference.
6. Confirm Key Vault naming and secret references.

## Planned first provisioning targets

1. Storage account with Table service enabled
2. Azure Table entities for:
   - `recipes`
   - `meal-plan-items`
   - `cook-progress`
3. Foundry project/model deployments for AI Tools backend integration

## Discussion gate

After this prep state, we should agree on:

1. resource naming and regions,
2. whether Foundry inference is hosted in the same region as the API,
3. initial model choice and deployment SKU,
4. rollout order (`dev` first, then `staging`).
