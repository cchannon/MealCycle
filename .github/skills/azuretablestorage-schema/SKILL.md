---
name: azuretablestorage-schema
description: "Azure Table Storage modeling and repository patterns for this stack. Use when designing partition/row keys, table entities, repository queries, concurrency handling with ETags, or data evolution strategy. Triggers: table storage schema, partition key, row key, ITableEntity, Azure.Data.Tables, query optimisation, soft delete."
---

# Table Storage Schema Patterns

## When to Use
- Designing a new Table Storage entity and key strategy
- Writing `ITableEntity` persistence models and repository mappings
- Defining efficient query paths using `PartitionKey`/`RowKey`
- Implementing soft delete or audit timestamps
- Planning data-shape changes without relational migrations

## Procedure

### New Entity
1. Create the domain model in `Domain/` (no storage SDK dependency)
2. Create a persistence model in `Infrastructure/Data/Tables/` implementing `ITableEntity`
3. Define deterministic `PartitionKey` and `RowKey` values from the dominant read/write paths
4. Create repository mapping functions between domain model and table entity
5. Ensure the table exists at startup/deployment and validate key design against expected workloads

### Standard Entity Shape
Every entity includes:
- `PartitionKey` — groups records for efficient query and transaction scope
- `RowKey` — unique record identifier within a partition
- `DateTimeOffset? Timestamp` and `ETag` — managed by Table Storage for concurrency

Soft-deletable entities additionally include:
- `bool IsDeleted` — applied in repository-level query filters
- `DateTimeOffset? DeletedAt`

### Query Patterns
- Primary path: point lookup with `PartitionKey` + `RowKey`
- Secondary path: partition-scoped query with explicit filter and bounded page size
- Avoid cross-partition scans in request/response paths; move them to background jobs when unavoidable
- Denormalize read models instead of relying on joins

## References
- [Entity + repository template](./references/entity-config-template.md)
- [Data evolution checklist](./references/migration-checklist.md)
- [Common query patterns](./references/query-patterns.md)
