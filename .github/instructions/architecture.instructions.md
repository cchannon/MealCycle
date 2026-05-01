---
description: "Use when designing system architecture, producing C4 diagrams, writing Architecture Decision Records (ADRs), decomposing components, or defining service boundaries and data flows for our Azure-hosted monorepo stack."
---

## Architecture Standards

### C4 Model Levels
Document architecture at the appropriate level of detail:
- **Level 1 — System Context**: What is this system? Who uses it? What external systems does it integrate with?
- **Level 2 — Container**: What are the major deployable units (frontend SPA, API, Functions, database, queues)?
- **Level 3 — Component**: What are the internal building blocks of each container (controllers, services, repositories)?
- Level 4 (code-level) diagrams are optional and reserved for non-obvious complexity

Use Mermaid diagrams (`\`\`\`mermaid`) inline in documentation for all C4 levels — they render in GitHub and VS Code.

### Service Boundaries
- Frontend (SPA) communicates only with the backend API — never directly with Azure services or the database
- Backend API owns business logic and data access — Functions are for async/background work, not for replacing API endpoints
- Service Bus decouples producers from consumers — the API enqueues; Functions process
- Authentication is handled via Microsoft Entra ID tokens; frontend requests include bearer tokens and backend validates issuer/audience/claims

### Architecture Decision Records (ADRs)
Store ADRs in `docs/decisions/` as `NNNN-<slug>.md`. Template:

```
# NNNN: Title

**Date**: YYYY-MM-DD  
**Status**: Proposed | Accepted | Deprecated | Superseded by [NNNN]

## Context
What is the problem or situation that requires a decision?

## Decision
What have we decided to do?

## Rationale
Why this option over the alternatives? What alternatives were considered?

## Consequences
What becomes easier or harder as a result? What future decisions does this constrain?
```

### Data Flow Documentation
- Document the data flow for every integration touchpoint: inputs, outputs, error paths
- Sequence diagrams (Mermaid `sequenceDiagram`) for multi-step async flows (e.g., API request -> Service Bus message -> Function processing -> Table Storage upsert)

### Security Architecture
- Document authentication and authorisation flows explicitly — who issues tokens, who validates them
- Identify all trust boundaries and what crosses them
- Data classification: note which entities contain PII and what protections apply

### Non-Functional Concerns to Address in Architecture
Every architecture document must cover:
- **Scalability**: What scales horizontally? What are the bottlenecks?
- **Availability**: What is the target SLA? What fails gracefully vs hard?
- **Observability**: How does an engineer diagnose a production issue? (App Insights, correlation IDs)
- **Security**: Auth, secrets management, network exposure
- **Cost**: Rough order-of-magnitude Azure cost estimate per environment tier

### What Architecture Docs Do NOT Include
- Sprint-level implementation decisions — those emerge during development
- Line-by-line technical specifications — those are produced after-the-fact as part of the sprint artefacts
