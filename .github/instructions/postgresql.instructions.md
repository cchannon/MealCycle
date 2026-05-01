---
description: "Use when writing repository code, table entity mappings, partition/row key design, or query patterns for Azure Storage Table persistence."
applyTo: "backend/**/Data/**, backend/**/Repositories/**, backend/**/*.cs"
---

## Azure Table Storage Standards

### Table & Key Design
- Use one table per aggregate or bounded context when practical; avoid over-fragmenting into many tiny tables
- Every entity must have deterministic `PartitionKey` and `RowKey` values based on access patterns
- Design for point reads (`PartitionKey` + `RowKey`) first; treat scans as exceptional
- Keep key components ASCII-safe and delimiter-consistent (`tenant|entityType`, `yyyyMMdd|id`)

### Entity Modeling
- Use `ITableEntity` implementations in Infrastructure only; map to Domain models at repository boundaries
- Include `Timestamp` and `ETag` on persisted entities for optimistic concurrency
- Avoid deep object graphs; flatten entities for storage and compose richer models in the application layer

### Versioning & Evolution
- There are no EF migrations; version entity shape with additive changes where possible
- For breaking changes, run dual-read/dual-write during transition and backfill asynchronously
- Keep a `SchemaVersion` property on entities that need explicit migration logic

### Queries & Performance
- Prefer `GetEntityAsync` for point reads and `QueryAsync` scoped to a partition for range access
- Avoid unbounded table scans; always cap result sets and continue with continuation tokens
- Keep hot partitions under control by distributing writes across key space when high write volume is expected
- Denormalise for read efficiency; Table Storage does not support joins

### Deletion & Retention
- Prefer soft delete flags for user-owned records unless hard deletion is explicitly required
- Record `DeletedAtUtc` for auditability and downstream purge jobs
- Use scheduled Functions for retention cleanup and archival workflows

### Timestamps
- All entities include `CreatedAtUtc` and `UpdatedAtUtc` where domain semantics require them
- Persist timestamps in UTC ISO-8601 compatible formats

### Connections & Security
- Storage account connection details resolve from Key Vault or managed identity-based endpoint config
- Use `DefaultAzureCredential` and RBAC for app-to-storage authentication where possible
- Never expose storage keys to frontend clients; issue scoped SAS only for explicit short-lived delegation cases
